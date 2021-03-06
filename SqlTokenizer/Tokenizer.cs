﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SqlTokenizer
{
    public class Tokenizer
    {
        #region Fields 

        private StringReader reader;
        private Queue<Token> queue;
        private StringBuilder buffer;
        private char currentChar;

        private string[] keywords = {
            "SELECT", "FROM", "WHERE", "UNION", "ORDER", "BY", "AS", "ALL", "DISTINCT", "CAST", "YEAR",
            "MONTH", "DAY", "DATEPART", "AND", "OR", "NOLOCK", "INSERT", "VALUES", "INTO", "UPDATE", "SET", "DELETE", "VARCHAR",
            "NUMERIC", "DATETIME", "CHAR","MAX", "MIN", "COUNT", "HAVING", "CASE", "WHEN", "THEN", "ELSE", "END", "IN", "EXISTS",
            "ISNULL", "SUM", "AVG", "TOP", "SCOPE_IDENTITY", "INT", "CONVERT", "BETWEEN", "GETDATE", "INNER", "JOIN", "OUTER",
            "LEFT", "RIGHT", "DATEADD", "LIKE", "ON", "IS", "NULL", "DESC", "ASC", "CROSS", "NOT", "EXISTS", "WITH", "GROUP", "ALL"
        };

        private bool End
        {
            get { return currentChar == '\0'; }
        }

        #endregion

        public Tokenizer(string source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            this.queue = new Queue<Token>();
            this.buffer = new StringBuilder();
            this.reader = new StringReader(source);
            this.ReadNextChar();
        }

        #region Public Methods

        public Queue<Token> GetTokens()
        {
            while (!End)
            {
                var token = this.ReadToken();

                if (token != null)
                    this.queue.Enqueue(token);
            }

            return this.queue;
        }

        #endregion

        #region Private Methods

        private Token ReadToken()
        {
            if (char.IsLetter(currentChar))
                return this.ReadWord();

            if (char.IsNumber(currentChar))
                return this.ReadNumber();

            if (currentChar == '\'')
                return this.ReadString();

            if (currentChar == '@')
                return this.ReadVariable();

            var token = this.ReadSymbol();

            return token;
        }

        private void SkipWhiteSpaces()
        {
            while (char.IsWhiteSpace(currentChar))
            {
                this.ReadNextChar();
            }
        }

        private Token ReadSymbol()
        {
            switch (currentChar)
            {
                case '+':
                case '-':
                case '*':
                case '/':
                case '^':
                case '(':
                case ')':
                case '[':
                case ']':
                case ';':
                case ':':
                case '=':
                case '.':
                case ',':
                    var token = new Token { Value = currentChar.ToString(), Type = TokenType.Symbol };
                    this.buffer.Clear();
                    this.ReadNextChar();
                    return token;
                default:
                    this.ReadNextChar();
                    break;
            }

            return null;
        }

        private Token ReadString()
        {
            this.buffer.Append(currentChar);
            this.ReadNextChar();

            while (currentChar != '\'')
            {
                this.buffer.Append(currentChar);
                this.ReadNextChar();
            }

            this.buffer.Append(currentChar);
            this.ReadNextChar();

            var token = new Token
            {
                Value = this.buffer.ToString(),
                Type = TokenType.String
            };

            this.buffer.Clear();

            return token;
        }

        private Token ReadNumber()
        {
            while (char.IsNumber(currentChar) || currentChar == '.')
            {
                this.buffer.Append(currentChar);
                this.ReadNextChar();
            }

            var token = new Token
            {
                Value = this.buffer.ToString(),
                Type = TokenType.Number
            };

            this.buffer.Clear();

            return token;
        }

        private Token ReadWord()
        {
            while (char.IsLetterOrDigit(currentChar) || currentChar == '_' || currentChar == '.')
            {
                this.buffer.Append(currentChar);
                this.ReadNextChar();
            }

            var token = new Token
            {
                Value = this.buffer.ToString(),
                Type = TokenType.Word
            };

            this.buffer.Clear();

            return token;
        }

        private Token ReadVariable()
        {
            while (char.IsLetterOrDigit(currentChar) || currentChar == '_' || currentChar == '@')
            {
                this.buffer.Append(currentChar);
                this.ReadNextChar();
            }

            var token = new Token
            {
                Value = this.buffer.ToString(),
                Type = TokenType.Variable
            };

            this.buffer.Clear();

            return token;
        }

        private void ReadNextChar()
        {
            var charCode = this.reader.Read();
            currentChar = charCode > 0 ? (char)charCode : '\0';
        }

        #endregion
    }
}