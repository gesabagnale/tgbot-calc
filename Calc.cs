using System;
using System.Collections.Generic;
using System.Text;

namespace tgbot_calc
{
    public enum LexemeType
    {
        LEFT_BRACKET, RIGHT_BRACKET,
        OP_PLUS, OP_MINUS, OP_MUL, OP_DIV,
        NUMBER,
        EOF,
    }

    public class Lexeme
    {
        public LexemeType type;
        public string value;

        public Lexeme(LexemeType type, string value)
        {
            this.type = type;
            this.value = value;
        }

        public Lexeme(LexemeType type, char value)
        {
            this.type = type;
            this.value = toString(value);
        }

        public string toString(char value)
        {
            return "Lexeme{" +
                    "type=" + type +
                    ", value='" + value + '\'' +
                    '}';
        }
    }

    public class LexemeBuffer
    {
        private int pos;

        public List<Lexeme> lexemes;

        public LexemeBuffer(List<Lexeme> lexemes)
        {
            this.lexemes = lexemes;
        }

        public Lexeme next()
        {
            return lexemes[pos++];
        }

        public void back()
        {
            pos--;
        }

        public int getPos()
        {
            return pos;
        }
    }

    class Calc
    {
        public static List<Lexeme> lexAnalyze(String expText)
        {
            List<Lexeme> lexemes = new List<Lexeme>();
            int pos = 0;
            while (pos < expText.Length)
            {
                char c = expText[pos];
                switch (c)
                {
                    case '(':
                        lexemes.Add(new Lexeme(LexemeType.LEFT_BRACKET, c));
                        pos++;
                        continue;
                    case ')':
                        lexemes.Add(new Lexeme(LexemeType.RIGHT_BRACKET, c));
                        pos++;
                        continue;
                    case '+':
                        lexemes.Add(new Lexeme(LexemeType.OP_PLUS, c));
                        pos++;
                        continue;
                    case '-':
                        lexemes.Add(new Lexeme(LexemeType.OP_MINUS, c));
                        pos++;
                        continue;
                    case '*':
                        lexemes.Add(new Lexeme(LexemeType.OP_MUL, c));
                        pos++;
                        continue;
                    case '/':
                        {
                            lexemes.Add(new Lexeme(LexemeType.OP_DIV, c));
                            pos++;
                            continue;
                        }
                    default:
                        if (c <= '9' && c >= '0')
                        {
                            StringBuilder sb = new StringBuilder();

                            do
                            {
                                sb.Append(c);
                                pos++;
                                if (pos >= expText.Length)
                                {
                                    break;
                                }
                                c = expText[pos];
                            } while (c <= '9' && c >= '0');
                            lexemes.Add(new Lexeme(LexemeType.NUMBER, sb.ToString()));
                        }
                        else
                        {
                            if (c != ' ')
                            {
                                throw new InvalidOperationException($"Unexpected character: {c}");
                            }
                            pos++;
                        }
                        break;
                }
            }
            lexemes.Add(new Lexeme(LexemeType.EOF, ""));
            return lexemes;
        }

        public static int expr(LexemeBuffer lexemes)
        {
            Lexeme lexeme = lexemes.next();
            if (lexeme.type == LexemeType.EOF)
            {
                return 0;
            }
            else
            {
                lexemes.back();
                return plusminus(lexemes);
            }
        }

        public static int plusminus(LexemeBuffer lexemes)
        {
            int value = multdiv(lexemes);
            while (true)
            {
                Lexeme lexeme = lexemes.next();
                switch (lexeme.type)
                {
                    case LexemeType.OP_PLUS:
                        value += multdiv(lexemes);
                        break;
                    case LexemeType.OP_MINUS:
                        value -= multdiv(lexemes);
                        break;
                    case LexemeType.EOF:
                    case LexemeType.RIGHT_BRACKET:
                        lexemes.back();
                        return value;
                    default:
                        throw new InvalidOperationException($"Unexpected token: {lexeme.value} at position: {lexemes.getPos()}");
                }
            }
        }

        public static int multdiv(LexemeBuffer lexemes)
        {
            int value = factor(lexemes);
            while (true)
            {
                Lexeme lexeme = lexemes.next();
                switch (lexeme.type)
                {
                    case LexemeType.OP_MUL:
                        value *= factor(lexemes);
                        break;
                    case LexemeType.OP_DIV:
                        value /= factor(lexemes);
                        break;
                    case LexemeType.EOF:
                    case LexemeType.RIGHT_BRACKET:
                    case LexemeType.OP_PLUS:
                    case LexemeType.OP_MINUS:
                        lexemes.back();
                        return value;
                    default:
                        throw new InvalidOperationException($"Unexpected token: {lexeme.value} at position: {lexemes.getPos()}");
                }
            }
        }

        public static int factor(LexemeBuffer lexemes)
        {
            Lexeme lexeme = lexemes.next();
            switch (lexeme.type)
            {
                case LexemeType.OP_MINUS:
                    int value_ = factor(lexemes);
                    return -value_;

                case LexemeType.NUMBER:
                    return Int32.Parse(lexeme.value);

                case LexemeType.LEFT_BRACKET:
                    int value = plusminus(lexemes);
                    lexeme = lexemes.next();
                    if (lexeme.type != LexemeType.RIGHT_BRACKET)
                    {
                        throw new InvalidOperationException($"Unexpected token: {lexeme.value} at position: {lexemes.getPos()}");
                    }
                    return value;
                default:
                    throw new InvalidOperationException($"Unexpected token: {lexeme.value} at position: {lexemes.getPos()}");
            }
        }

        public static int calculate(string expressionText)
        {
            List<Lexeme> lexemes = lexAnalyze(expressionText);
            LexemeBuffer lexemeBuffer = new LexemeBuffer(lexemes);

            return expr(lexemeBuffer);
        }


    }
}
