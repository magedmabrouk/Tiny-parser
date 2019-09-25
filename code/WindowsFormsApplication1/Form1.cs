using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        
        char[] letters = new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
        char[] nums = new char[] { '1', '2', '3', '4', '5', '6', '7', '8', '9', '0' };
        bool error = false;
        int Char_itr = 0;

        List<string> Tokens = new List<string>() /*{"x", ":=", "3", ";", "y", ":=", "4", ";", "m", ":=", "x",";","hoba", ":=", "5", "end" }*/;
        List<string> TokensClassification = new List<string>() /*{ "Identifier", ":=", "Number", ";", "Identifier", ":=", "Number", ";", "Identifier", ":=", "Identifier",";","Identifier",":=","Number", "end" }*/;
        List<string> graph_text = new List<string>();
        List<List<int>> graph_nodes = new List<List<int>>()  /*{ new List<int> { }, new List<int> { 2, 3 }, new List<int> { 4 }, new List<int> { 5, 6, 0, 7 }, new List<int> { }, new List<int> { }, new List<int> { }, new List<int> { }}*/;
        
        //For drawings
        int HeadOfGraph;
        float PanelMax_X_ExpansionSoFar = 0;
        int TextBoxNumOfLines = 0;
        
        
        //SCANNER
        public bool IsLetter(char c)
        {
            for (int i = 0; i < 26; i++)
            {
                if (letters[i] == c) return true;
            }
            return false;
        }

        public bool IsNum(char c)
        {
            for (int i = 0; i < 10; i++)
            {
                if (nums[i] == c) return true;
            }
            return false;
        }

        public bool IsNewToken(char c)
        {
            if (c == ';' || c == ' ')
                return true;
            return false;
        }

        public void navigate(ref int x, ref char y, int state)
        {
            if (state == 6) { y = 'f'; x = 0; }
            else if (state == 7) { y = 'h'; x = 15; }
            else if (state == 15) { y = 'e'; x = 16; }
            else if (state == 16) { y = 'n'; x = 0; }
            else if (state == 18) { y = 's'; x = 19; }
            else if (state == 19) { y = 'e'; x = 0; }
            else if (state == 21) { y = 'd'; x = 0; }
            else if (state == 9) { y = 'e'; x = 23; }
            else if (state == 24) { y = 'e'; x = 25; }
            else if (state == 25) { y = 'a'; x = 26; }
            else if (state == 26) { y = 't'; x = 0; }
            else if (state == 28) { y = 'd'; x = 0; }
            else if (state == 10) { y = 'n'; x = 30; }
            else if (state == 30) { y = 't'; x = 31; }
            else if (state == 31) { y = 'i'; x = 32; }
            else if (state == 32) { y = 'l'; x = 0; }
            else if (state == 11) { y = 'r'; x = 34; }
            else if (state == 34) { y = 'i'; x = 35; }
            else if (state == 35) { y = 't'; x = 36; }
            else if (state == 36) { y = 'e'; x = 0; }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            panel1.Size = new Size(484, 429);
            for (int u = 0; u < 2; u++)
            {
                PanelMax_X_ExpansionSoFar = 0;
                draw_rec(0, 0, panel1.Size.Width, panel1.Size.Width);
                Char_itr = 0;
                error = false;
                graph_nodes.Clear();
                graph_text.Clear();
                TextBoxNumOfLines = textBox1.Lines.Count();
                string code = "";
                for (int j = 0; j < TextBoxNumOfLines; j++)
                {
                    code += textBox1.Lines[j] + " ";
                }
                int i = 0;                // counter 
                string token = "";
                Tokens.Clear();
                TokensClassification.Clear();
                int state = 1;
                int x = 0;
                char y = '0';
                while (i != code.Length)
                {

                    navigate(ref x, ref y, state);

                    switch (state)
                    {
                        case 1:    //starting state which read  the first char and decides the path 
                            token = "";
                            token += code[i];
                            if (code[i] == '+' || code[i] == '-' || code[i] == '*' || code[i] == '/') state = 2;  // arthematic operations 
                            else if (code[i] == '=' || code[i] == '<' || code[i] == '>') state = 3; //comparison
                            else if (code[i] == '(' || code[i] == ')') state = 4;  //special sympol
                            else if (code[i] == ':') state = 5;  //leads to the assign sympol ":="
                            else if (code[i] == 'i') state = 6;  //leads to the word "if" or any other identifier that begins with "i" 
                            else if (code[i] == 't') state = 7;  //leads to the word "then" or any other identifier that begins with "t" 
                            else if (code[i] == 'e') state = 8;  //leads to the words "else", "end" or any other identifier that begins with "e" 
                            else if (code[i] == 'r') state = 9;  //leads to the words "read", "repeat" or any other identifier that begins with "r" 
                            else if (code[i] == 'u') state = 10;  //leads to the word "until" or any other identifier that begins with "u" 
                            else if (code[i] == 'w') state = 11;  //leads to the word "write" or any other identifier that begins with "w" 
                            else if (IsLetter(code[i])) state = 12;  //for identifiers 
                            else if (IsNum(code[i])) state = 13;  // for numbers
                            else if (code[i] == '{') state = -1;   // for comments
                            else if (code[i] == ';') { token = ""; Tokens.Add(";"); TokensClassification.Add(";"); }
                            break;

                        case 2:
                            if (IsNewToken(code[i]))
                            {
                                if (token == "+") TokensClassification.Add("+");
                                if (token == "-") TokensClassification.Add("-");
                                if (token == "*") TokensClassification.Add("*");
                                if (token == "/") TokensClassification.Add("/");
                                Tokens.Add(token); state = 1;
                                if (code[i] == ';') { Tokens.Add(";"); TokensClassification.Add(";"); }
                            }
                            else { token += code[i]; state = 100; }
                            break;

                        case 3:
                            if (IsNewToken(code[i]))
                            {
                                if (token == "<") TokensClassification.Add("<");
                                if (token == "=") TokensClassification.Add("=");
                                if (token == ">") TokensClassification.Add(">");
                                Tokens.Add(token); state = 1;
                                if (code[i] == ';') { Tokens.Add(";"); TokensClassification.Add(";"); }
                            }
                            else { token += code[i]; state = 100; }
                            break;

                        case 4:
                            if (IsNewToken(code[i]))
                            {
                                if (token == "(") TokensClassification.Add("(");
                                if (token == ")") TokensClassification.Add(")");
                                Tokens.Add(token); state = 1;
                                if (code[i] == ';') { Tokens.Add(";"); TokensClassification.Add(";"); }
                            }
                            else { token += code[i]; state = 100; }
                            break;

                        case 5:
                            if (code[i] == '=') { token += code[i]; state = 40; }
                            else if (IsNewToken(code[i])) { Tokens.Add(token); TokensClassification.Add("Unrecognized"); state = 1; if (code[i] == ';') { Tokens.Add(";"); TokensClassification.Add(";"); } }
                            else { token += code[i]; state = 100; }
                            break;

                        case 40:
                            if (IsNewToken(code[i])) { Tokens.Add(token); TokensClassification.Add(":="); state = 1; if (code[i] == ';') { Tokens.Add(";"); TokensClassification.Add(";"); } }
                            else { token += code[i]; state = 100; }
                            break;

                        case 6:
                        case 7:
                        case 9:
                        case 10:
                        case 11:
                        case 15:
                        case 16:
                        case 18:
                        case 19:
                        case 21:
                        case 24:
                        case 25:
                        case 26:
                        case 28:
                        case 30:
                        case 31:
                        case 32:
                        case 34:
                        case 35:
                        case 36:
                            if (IsNewToken(code[i])) { Tokens.Add(token); TokensClassification.Add("Identifier"); if (code[i] == ';') { Tokens.Add(";"); TokensClassification.Add(";"); } state = 1; break; }
                            token += code[i];
                            if (code[i] == y) state = x;
                            else if (IsLetter(code[i])) state = 12;
                            else state = 100;
                            break;

                        case 8:
                            if (IsNewToken(code[i])) { Tokens.Add(token); TokensClassification.Add("Identifier"); if (code[i] == ';') { Tokens.Add(";"); TokensClassification.Add(";"); } state = 1; break; }
                            token += code[i];
                            if (code[i] == 'l') state = 18;
                            else if (code[i] == 'n') state = 21;
                            else if (IsLetter(code[i])) state = 12;
                            else state = 100;
                            break;

                        case 23:
                            if (IsNewToken(code[i])) { Tokens.Add(token); TokensClassification.Add("Identifier"); state = 1; if (code[i] == ';') { Tokens.Add(";"); TokensClassification.Add(";"); } break; }
                            token += code[i];
                            if (code[i] == 'p') state = 24;
                            else if (code[i] == 'a') state = 28;
                            else if (IsLetter(code[i])) state = 12;
                            else state = 100;
                            break;

                        case 12:
                            if (IsNewToken(code[i])) { Tokens.Add(token); TokensClassification.Add("Identifier"); state = 1; if (code[i] == ';') { Tokens.Add(";"); TokensClassification.Add(";"); } break; }
                            token += code[i];
                            if (!IsNewToken(code[i]) && !IsLetter(code[i])) state = 100;
                            break;

                        case 13:
                            if (IsNewToken(code[i])) { Tokens.Add(token); TokensClassification.Add("Number"); state = 1; if (code[i] == ';') { Tokens.Add(";"); TokensClassification.Add(";"); } break; }
                            token += code[i];
                            if (!IsNewToken(code[i]) && !IsNum(code[i])) state = 100;
                            break;

                        case 0:   // transient state between considering the token a reserved word or an identifier
                            if (IsNewToken(code[i]))
                            {
                                if (token == "if") TokensClassification.Add("if");
                                if (token == "then") TokensClassification.Add("then");
                                if (token == "else") TokensClassification.Add("else");
                                if (token == "end") TokensClassification.Add("end");
                                if (token == "read") TokensClassification.Add("read");
                                if (token == "repeat") TokensClassification.Add("repeat");
                                if (token == "until") TokensClassification.Add("until");
                                if (token == "write") TokensClassification.Add("write");
                                Tokens.Add(token); state = 1;
                                if (code[i] == ';') { Tokens.Add(";"); TokensClassification.Add(";"); }
                            }
                            else if (IsLetter(code[i])) { token += code[i]; state = 12; }
                            else { token += code[i]; state = 100; }
                            break;

                        case 100: //the token is not part of the tiny language lexical conventions
                            if (IsNewToken(code[i])) { Tokens.Add(token); TokensClassification.Add("Unrecognized"); state = 1; if (code[i] == ';') { Tokens.Add(";"); TokensClassification.Add(";"); } }
                            token += code[i];
                            break;

                        case -1:
                            if (code[i] == '}') state = 1;
                            break;

                    }

                    i++;

                }
                Tokens.Add("end"); TokensClassification.Add("end");
                for (int j = 0; j < Tokens.Count; j++)
                {
                    Console.WriteLine(Tokens[j] + "  " + TokensClassification[j]);
                }
                int m = stmt_seq();
                Console.WriteLine("yesssss");
                Console.WriteLine(graph_nodes.Count().ToString());
                Console.WriteLine(m.ToString());
                foreach (List<int> s in graph_nodes)
                {
                    Console.WriteLine(graph_nodes.IndexOf(s).ToString() + "     " + graph_text[graph_nodes.IndexOf(s)]);
                    foreach (int j in s)
                    {
                        Console.Write("   " + j.ToString() + "," + graph_text[j]);
                    }
                    Console.WriteLine("");
                }
                draw_from_head();
            }

        }



        //PARSER
        public int stmt_seq()
        {
            int temp = stmt();
            int temp2 = temp;
            while (TokensClassification[Char_itr] == ";")
            {
                match(TokensClassification[Char_itr]);
                int temp3 = stmt(); 
                graph_nodes[temp2].Add(temp3);
                graph_text[temp3] = "_SIB"+graph_text[temp3];
                temp2 = temp3;
            }
            return temp;
        }

        public int stmt()
        {
            int temp=0;
            if (TokensClassification[Char_itr] == "if") temp = if_stmt();
            else if (TokensClassification[Char_itr] == "Identifier") temp = ass_stmt();
            else if (TokensClassification[Char_itr] == "write") temp = write_stmt();
            else if (TokensClassification[Char_itr] == "read") temp = read_stmt();
            else if (TokensClassification[Char_itr] == "repeat") temp = repeat_stmt();
            else error = true;
            return temp;
        }

        public int if_stmt()
        {
            List<int> new_node = new List<int>();
            graph_nodes.Add(new_node);
            graph_text.Add(Tokens[Char_itr]);
            match(TokensClassification[Char_itr]);
            int temp = graph_nodes.IndexOf(new_node);
            graph_nodes[temp].Add(exp());
            match("then");
            graph_nodes[temp].Add(stmt_seq());
            if( TokensClassification[Char_itr] == "else")
            {
                match("else");
                graph_nodes[temp].Add(stmt_seq());
            }
            match("end");
            return temp;
        }

        public int repeat_stmt()
        {
            List<int> new_node = new List<int>();
            graph_nodes.Add(new_node);
            graph_text.Add(Tokens[Char_itr]);
            match(TokensClassification[Char_itr]);
            int temp = graph_nodes.IndexOf(new_node);
            graph_nodes[temp].Add(stmt_seq());
            match("until");
            graph_nodes[temp].Add(exp());
            return temp;
        }

        public int ass_stmt()
        {
            List<int> new_node = new List<int>();
            graph_nodes.Add(new_node);
            graph_text.Add("assign "+Tokens[Char_itr]);
            match(TokensClassification[Char_itr]);
            match(":=");
            int temp = graph_nodes.IndexOf(new_node);
            graph_nodes[temp].Add(exp());
            return temp;
        }

        public int read_stmt()
        {
            List<int> new_node = new List<int>();
            graph_nodes.Add(new_node);
            match(TokensClassification[Char_itr]);
            graph_text.Add("read " + Tokens[Char_itr]);
            match("Identifier");
            int temp = graph_nodes.IndexOf(new_node);
            return temp;
        }

        public int write_stmt()
        {
            List<int> new_node = new List<int>();
            graph_nodes.Add(new_node);
            graph_text.Add("write");
            match(TokensClassification[Char_itr]);
            int temp = graph_nodes.IndexOf(new_node);
            graph_nodes[temp].Add(exp());
            return temp;
        }

        public int exp()
        {
            int temp = simple_exp();
            if (TokensClassification[Char_itr] == "<" || TokensClassification[Char_itr] == "=")
            {
                List<int> new_node = new List<int> { temp };
                graph_nodes.Add(new_node);
                graph_text.Add("op " + Tokens[Char_itr]);
                match(TokensClassification[Char_itr]);
                temp = graph_nodes.IndexOf(new_node);
                graph_nodes[temp].Add(simple_exp());
            }
            return temp;
        }

        public int simple_exp()
        {
            int temp = term();
            while (TokensClassification[Char_itr] == "+" || TokensClassification[Char_itr] == "-")
            {
                List<int> new_node = new List<int> { temp };
                graph_nodes.Add(new_node);
                graph_text.Add("op "+Tokens[Char_itr]);
                match(TokensClassification[Char_itr]);
                temp = graph_nodes.IndexOf(new_node);
                graph_nodes[temp].Add(term());
            }
            return temp;
        }

        public int term()
        {
            int temp = factor();
            while (TokensClassification[Char_itr] == "*")
            {
                List<int> new_node = new List<int> { temp };
                graph_nodes.Add(new_node);
                graph_text.Add("op "+Tokens[Char_itr]);
                match(TokensClassification[Char_itr]);
                temp = graph_nodes.IndexOf(new_node);
                graph_nodes[temp].Add(factor());
            }
            return temp;
        }

        public int factor()
        {
            int temp=0;
            if (TokensClassification[Char_itr] == "(")
            {
                match("(");
                temp = exp();
                match(")");
            }
            else if (TokensClassification[Char_itr] == "Number" || TokensClassification[Char_itr] == "Identifier")
            {
                List<int> new_node = new List<int> { };
                graph_nodes.Add(new_node);
                graph_text.Add(Tokens[Char_itr]);
                temp = graph_nodes.IndexOf(new_node);
                match(TokensClassification[Char_itr]);
            }
            else error = true;
            return temp;
        }

        public void match(string t)
        {
            if (TokensClassification[Char_itr] == t) Char_itr++;
            else error = true;
        }



        //DRAWING GRAPH
        //to initialize drawing the graph using recursion
        public void draw_from_head()
        {

            //searching for the head of graph that has no parent
            bool found;
            foreach (List<int> search in graph_nodes)
            {
                found = false;
                foreach (List<int> sublist in graph_nodes)
                {
                    foreach (int node in sublist)
                    {
                        if (graph_nodes.IndexOf(search) == node)
                        {
                            found = true;
                        }
                    }
                }
                if (!found) { HeadOfGraph = graph_nodes.IndexOf(search); break; }
            }

            float headWidth = (graph_text[HeadOfGraph].Length * 5.5f) + 10;
            draw_node(150 , 20, graph_text[HeadOfGraph], headWidth);
            //initializing graph recursion
            draw_graph(graph_nodes[HeadOfGraph], HeadOfGraph, 150, 20, 50, 50);
        }
 
        void draw_graph(List<int> children, int parent, float Parent_X_Position, float Parent_Y_Position, float ParentGenes, float SiblingsHorizontalExpand)
        {
            if ((panel1.Size.Width - 200) < Parent_X_Position) panel1.Size = new Size(panel1.Size.Width + 200, panel1.Size.Height);
            int sib = 0; int sons = children.Count();

            //Counting siblings that should be drawn to the right not underneath
            foreach (int child in children)
            {
                if (graph_text[child].Length > 4)
                {
                    if (graph_text[child].Substring(0, 4) == "_SIB") sib++;
                }
            }
            sons = sons - sib;

            float RateOfExpansion = 1;
            float ParentNodeWidth = (graph_text[parent].Length * 5.5f) + 10;
            foreach (int child in children)
            {
                //Detecting siblings to draw right 
                bool SiblingNotChild = false;
                if (graph_text[child].Length > 4)
                {
                    if (graph_text[child].Substring(0, 4) == "_SIB") { SiblingNotChild = true; graph_text[child]=graph_text[child].Substring(4); }
                }
                float ChildWidth = (graph_text[child].Length * 5.5f) + 10;

                if (SiblingNotChild)
                {
                    SiblingsHorizontalExpand *= 1.1f;
                    float Child_X_Position = Parent_X_Position + ParentNodeWidth + 25 + (RateOfExpansion * SiblingsHorizontalExpand * (float)Math.Cos(0));
                    if (Child_X_Position < PanelMax_X_ExpansionSoFar) { Child_X_Position = PanelMax_X_ExpansionSoFar + 35; PanelMax_X_ExpansionSoFar = Child_X_Position; } else PanelMax_X_ExpansionSoFar = Child_X_Position;
                    float Child_Y_Position = Parent_Y_Position + 10 + (RateOfExpansion * ParentGenes * (float)Math.Sin(0));
                    draw_edge(Parent_X_Position + ParentNodeWidth, Parent_Y_Position + 10, Child_X_Position, Child_Y_Position);
                    draw_node(Child_X_Position, Child_Y_Position-10, graph_text[child],ChildWidth);
                    draw_graph(graph_nodes[child], child, Child_X_Position, Child_Y_Position-10, RateOfExpansion * ParentGenes, SiblingsHorizontalExpand);
                }

                
                else if (children.IndexOf(child) < sons / 2)
                {
                    float Child_X_Position = Parent_X_Position + (ParentNodeWidth / 2) - (((RateOfExpansion * ParentGenes) + 5) * (float)Math.Cos(15 * Math.PI / 180 * (children.IndexOf(child) + 1)));
                    float Child_Y_Position = Parent_Y_Position + 20 + (((RateOfExpansion * ParentGenes) + 5) * (float)Math.Sin(15 * Math.PI / 180 * (children.IndexOf(child) + 1)));
                    draw_edge(Parent_X_Position + (ParentNodeWidth / 2), Parent_Y_Position + 20, Child_X_Position, Child_Y_Position);
                    draw_node(Child_X_Position - (ChildWidth / 2), Child_Y_Position, graph_text[child],ChildWidth );
                    draw_graph(graph_nodes[child], child, Child_X_Position - (ChildWidth / 2), Child_Y_Position, RateOfExpansion * ParentGenes, SiblingsHorizontalExpand);
                }
                else if ( sons%2==1 && children.IndexOf(child)==(sons-1)/2)
                {
                    float Child_X_Position = Parent_X_Position + (ParentNodeWidth / 2) - (((RateOfExpansion * ParentGenes) + 5) * (float)Math.Cos(90 * Math.PI / 180));
                    float Child_Y_Position = Parent_Y_Position + 20 + 20 + (((RateOfExpansion * ParentGenes) + 15) * (float)Math.Sin(90 * Math.PI / 180));
                    draw_edge(Parent_X_Position + (ParentNodeWidth / 2), Parent_Y_Position + 20, Child_X_Position, Child_Y_Position);
                    draw_node(Child_X_Position - (ChildWidth / 2), Child_Y_Position, graph_text[child], ChildWidth);
                    draw_graph(graph_nodes[child], child, Child_X_Position - (ChildWidth / 2), Child_Y_Position, RateOfExpansion * ParentGenes, SiblingsHorizontalExpand);
                }

                else
                {
                    float Child_X_Position = Parent_X_Position + (ParentNodeWidth / 2) + (RateOfExpansion * ParentGenes * (float)Math.Cos((30 * Math.PI / 180) * (sons - children.IndexOf(child))));
                    if (Child_X_Position < PanelMax_X_ExpansionSoFar) { Child_X_Position = PanelMax_X_ExpansionSoFar + 15; PanelMax_X_ExpansionSoFar = Child_X_Position; } else PanelMax_X_ExpansionSoFar = Child_X_Position;
                    float Child_Y_Position = Parent_Y_Position + 20 + (RateOfExpansion * ParentGenes * (float)Math.Sin((30 * Math.PI / 180) * (sons - children.IndexOf(child))));
                    draw_edge(Parent_X_Position + (ParentNodeWidth / 2), Parent_Y_Position + 20, Child_X_Position, Child_Y_Position);
                    draw_node(Child_X_Position - (ChildWidth / 2), Child_Y_Position, graph_text[child], ChildWidth);
                    draw_graph(graph_nodes[child], child, Child_X_Position - (ChildWidth / 2), Child_Y_Position, RateOfExpansion * ParentGenes, SiblingsHorizontalExpand);
                }
            }
        }

        public void draw_node(float x, float y, string text, float width)
        {
            SolidBrush b = new SolidBrush(Color.Red);
            SolidBrush bl = new SolidBrush(Color.Blue);
            Pen p = new Pen(b, 2);
            FontFamily ff = new FontFamily("Arial");
            System.Drawing.Font font = new System.Drawing.Font(ff, 8, FontStyle.Regular);
            Graphics g = panel1.CreateGraphics();
            g.DrawEllipse(p, x, y, width, 20);
            g.DrawString(text, font, bl, new PointF(x+5, y+4));
        }

        public void draw_edge(float x1, float y1, float x2, float y2)
        {
            SolidBrush b = new SolidBrush(Color.Red);
            Pen p = new Pen(b, 2);
            Graphics g = panel1.CreateGraphics();
            g.DrawLine(p, x1, y1, x2, y2);
        }

        public void draw_rec(float x1, float y1, float x2, float y2)
        {
            SolidBrush b = new SolidBrush(Color.LightGray);
            Pen p = new Pen(b, 2);
            Graphics g = panel1.CreateGraphics();
            g.FillRectangle(b, x1, y1, x2, y2);
        }



        public Form1()
        {
            InitializeComponent();
        }

        
    }
}
