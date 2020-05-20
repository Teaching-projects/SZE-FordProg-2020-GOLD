# What this project is all about?
The aim of this project is to generally get to know how parser generators work. Throughout my learning path I'm using a long forgotten but much as good LALR type parsing system called Grammar Oriented Language Developer aka GOLD (http://www.goldparser.org/download.htm).

# What are the aims of this project?
Besides the upper mentioned goals, my aim is to create a cross-platform (Windows/Linux/OSX) parser that can parse and debug grammars in "real-time" created by GOLD Builder (Windows-only).

#### Fine, but can't you test your grammar out-of-the-box inside the Builder?!
Yeah, that's true, however not in cross-platform manner (the Builder is only Windows compatible)... anyway it just wouldn't be that much fun.

## Goals: 
  * Porting and refactorization of the original .NET GOLD Engine's source code written in Visual Basic to C#
  * Correction of bugs
  * Modernization of the code as much as nowadays standards allow
  * Porting of the cleaned up code to .NET Core, creation of a cross-platform project
  * Creation of a universal parsing software that can parse EGT grammar files created by the Builder

## What is done:
  * Porting to C# and refactorization
  * Bugs are fixed
    * bugs related to the lexer fixed
    * software logic analyzed: everything is good to go
  * Modernization:
    * original ArrayList types changed to generic List<T> type
    * public fields and methods related to these fields changed to properties
    * unused types (e.g. IntegerList) wiped out and replaced with generic ones
    * debug mode added (only works within CLI apps)
  * Universal parsing app created

# Usage
  - Generate an EGT grammar file with GOLD Builder or get one from Grammars folder.
  - Download the parser app from releases tab.
  - Start the parser app from a Prompt/Terminal with [-g] switch followed by the absolute path of the grammar file.
  - Optional: Use [-s] switch followed by the length of the desired output size. Default length: 120 characters.
  - Type in the syntax that is waiting to be parsed.
  - Read the response and repeat as many times as you want.
  - Type "exit" for quit from the app.
  - For parsing a file use the [-f] switch.

# Example
### On Windows:
```
DriveLetter:\path-to-file\GOLD.Parser.exe -g "DriveLetter:\path-to-file\grammar.egt" -s 100

Type your syntax here: a+b

Symbol read: 'a';                Token created: 'Identifier';     On stack: ;
Shift token: 'Identifier';                                        On stack: 'Identifier';
Symbol read: '+';                Token created: '+';              On stack: 'Identifier';
Lookahead: '+';                  Reduce: 'Value';                 On stack: 'Value';
Lookahead: '+';                  Reduce: 'Negate Exp';            On stack: 'Negate Exp';
Lookahead: '+';                  Reduce: 'Mult Exp';              On stack: 'Mult Exp';
Lookahead: '+';                  Reduce: 'Expression';            On stack: 'Expression';
Shift token: '+';                                                 On stack: 'Expression' '+';
Symbol read: 'b';                Token created: 'Identifier';     On stack: 'Expression' '+';
Shift token: 'Identifier';                                        On stack: 'Expression' '+' 'I...
Symbol read: '';                 Token created: 'EOF';            On stack: 'Expression' '+' 'I...
Lookahead: 'EOF';                Reduce: 'Value';                 On stack: 'Expression' '+' 'V...
Lookahead: 'EOF';                Reduce: 'Negate Exp';            On stack: 'Expression' '+' 'N...
Lookahead: 'EOF';                Reduce: 'Mult Exp';              On stack: 'Expression' '+' 'M...
Lookahead: 'EOF';                Reduce: 'Expression';            On stack: 'Expression';
Lookahead: 'EOF';                Reduce: 'Program';               On stack: 'Program';
Accepted!


Type your syntax here: exit
```
### On Linux:
```
/path-to-file/GOLD.Parser -g "/path-to-file/grammar.egt"
Type your syntax here: a+b

Symbol read: 'a';                       Token created: 'Identifier';            On stack: ;
Shift token: 'Identifier';                                                      On stack: 'Identifier';
Symbol read: '+';                       Token created: '+';                     On stack: 'Identifier';
Lookahead: '+';                         Reduce: 'Value';                        On stack: 'Value';
Lookahead: '+';                         Reduce: 'Negate Exp';                   On stack: 'Negate Exp';
Lookahead: '+';                         Reduce: 'Mult Exp';                     On stack: 'Mult Exp';
Lookahead: '+';                         Reduce: 'Expression';                   On stack: 'Expression';
Shift token: '+';                                                               On stack: 'Expression' '+';
Symbol read: 'b';                       Token created: 'Identifier';            On stack: 'Expression' '+';
Shift token: 'Identifier';                                                      On stack: 'Expression' '+' 'Identifi...
Symbol read: '';                        Token created: 'EOF';                   On stack: 'Expression' '+' 'Identifi...
Lookahead: 'EOF';                       Reduce: 'Value';                        On stack: 'Expression' '+' 'Value';
Lookahead: 'EOF';                       Reduce: 'Negate Exp';                   On stack: 'Expression' '+' 'Negate E...
Lookahead: 'EOF';                       Reduce: 'Mult Exp';                     On stack: 'Expression' '+' 'Mult Exp';
Lookahead: 'EOF';                       Reduce: 'Expression';                   On stack: 'Expression';
Lookahead: 'EOF';                       Reduce: 'Program';                      On stack: 'Program';
Accepted!


Type your syntax here: exit
```

# During operation (demonstrational task)
This section demonstrates how the system works. Let's take the .obj format as an example (the complete RFC can be found here:
http://paulbourke.net/dataformats/obj/, much concise information can be read here: https://en.wikipedia.org/wiki/Wavefront_.obj_file).
For this demonstration I wrote the grammar below. It realizes only a small subset of the rules forming the original Wavefront object format.
```
"Start Symbol" = <Start>

! -------------------------------------------------
! Character Sets
! -------------------------------------------------

{WS}               = {Whitespace} - {CR} - {LF}
{NoSpcPrint}       = {Printable} - {Space}        

! -------------------------------------------------
! Terminals
! -------------------------------------------------

Whitespace    = {WS}+           
NewLine       = {CR}{LF} | {CR} | {LF}             
Comm          = '#'{Printable}*             
Float         = '-'?{Number}('.'{Number}+|'.'{Number}+'e''-'?{Number}+)?{Space}*                         
Simplet       = '-'?{Number}+{Space}'-'?{Number}+{Space}'-'?{Number}+({Space}'-'?{Number}+)*             
Duplet        = '-'?{Number}+[/]'-'?{Number}+{Space}'-'?{Number}+[/]'-'?{Number}+{Space}'-'?{Number}+[/]'-'?{Number}+({Space}'-'?{Number}+[/]'-'?{Number}+)*             
Triplet       = '-'?{Number}+[/]('-'{Number}*|{Number}*)[/]'-'?{Number}+{Space}'-'{Number}+'/]'('-'{Number}*|{Number}*)'/''-'?{Number}+{Space}'-'?{Number}+'/'('-'{Number}*|{Number}*)'/''-'?{Number}+({Space}'-'?{Number}+'/'('-'{Number}*|{Number}*)'/''-'?{Number}+)*
V             = 'v'{Space}             
VP            = 'vp'{Space}              
VN            = 'vn'{Space}            
VT            = 'vt'{Space}                                  
F             = 'f'{Space}             
L             = 'l'{Space}             
P             = 'p'{Space}             
O             = 'o' | 'o'{Space}{NoSpcPrint}+             
G             = 'g' | 'g'{Space}{NoSpcPrint}+            
S             = 's' | 's'{Space}{NoSpcPrint}+            
MTLLib        = 'mtllib'{Space}{Printable}+            
UseMTL        = 'usemtl'{Space}{NoSpcPrint}+

! -------------------------------------------------
! Rules
! -------------------------------------------------

<nl> ::= NewLine <nl>
      |  !Empty

<Comment> ::= Comm  
     
<Geovertex> ::= V Float Float Float
             |  V Float Float Float Float
             
<TextureCoordinate> ::= VT Float
                     | VT Simplet
                     | VT Float Float Float
             
<VertexNormal> ::= VN Float Float Float
                
<SpaceVertex> ::= VP Float
               | VP Simplet
               | VP Float Float
               | VP Float Float Float
               
<Face> ::= F Simplet 
        | F Duplet 
        | F Triplet
        
<Line> ::= L Simplet
        | L Duplet 
        
<Point> ::= P Simplet
         
<Materials> ::= MTLLib
             
<UseMaterial> ::= UseMTL
               
<Object> ::= O
          
<Group> ::= G
         
<Smoothing> ::= S 

<Start> ::= <nl> <Program>

<Program> ::= <Comment> <nl> <Program>
           | <Geovertex> <nl> <Program>
           | <TextureCoordinate> <nl> <Program>
           | <VertexNormal> <nl> <Program>
           | <SpaceVertex> <nl> <Program>
           | <Face> <nl> <Program>
           | <Line> <nl> <Program>
           | <Point> <nl> <Program>
           | <Materials> <nl> <Program>
           | <UseMaterial> <nl> <Program>
           | <Object> <nl> <Program>
           | <Group> <nl> <Program>
           | <Smoothing> <nl> <Program>
           | !Empty
```
After finishing the grammar using the Builder I generated the EGT file (can be found inside Grammars folder). For testing purposes I downloaded a paper plane model from here: https://free3d.com/3d-model/paper-airplane-58536.html (can be found in TestFiles folder). In the end, I used the Parser the following way:
```
.\GOLD.Parser.exe -g "OBJ.egt" -s 300 -f "paper_plane.obj"
```
The parser accepted the file format with the following output:
```
Symbol read: '# WaveFront *.obj file (generated by CINEMA 4D)';   Token created: 'Comm';                                        On stack: ;
Lookahead: 'Comm';                                                Reduce: '<nl> ::= ';                                          On stack: 'nl';
Shift token: 'Comm';                                                                                                            On stack: 'nl' 'Comm';
Symbol read: 'LF';                                                Token created: 'NewLine';                                     On stack: 'nl' 'Comm';
Lookahead: 'NewLine';                                             Reduce: '<Comment> ::= Comm';                                 On stack: 'nl' 'Comment';
Shift token: 'NewLine';                                                                                                         On stack: 'nl' 'Comment' 'NewLine';
Symbol read: 'LF';                                                Token created: 'NewLine';                                     On stack: 'nl' 'Comment' 'NewLine';
Shift token: 'NewLine';                                                                                                         On stack: 'nl' 'Comment' 'NewLine' 'NewLine';
Symbol read: 'g paper_airplane';                                  Token created: 'G';                                           On stack: 'nl' 'Comment' 'NewLine' 'NewLine';
Lookahead: 'G';                                                   Reduce: '<nl> ::= ';                                          On stack: 'nl' 'Comment' 'NewLine' 'NewLine' 'nl';
Lookahead: 'G';                                                   Reduce: '<nl> ::= NewLine <nl>';                              On stack: 'nl' 'Comment' 'NewLine' 'nl';
Lookahead: 'G';                                                   Reduce: '<nl> ::= NewLine <nl>';                              On stack: 'nl' 'Comment' 'nl';
Shift token: 'G';                                                                                                               On stack: 'nl' 'Comment' 'nl' 'G';
Symbol read: 'LF';                                                Token created: 'NewLine';                                     On stack: 'nl' 'Comment' 'nl' 'G';
Lookahead: 'NewLine';                                             Reduce: '<Group> ::= G';                                      On stack: 'nl' 'Comment' 'nl' 'Group';
Shift token: 'NewLine';                                                                                                         On stack: 'nl' 'Comment' 'nl' 'Group' 'NewLine';
...
Lookahead: 'V';                                                   Reduce: '<nl> ::= NewLine <nl>';                              On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Geovertex' 'nl';
Shift token: 'V';                                                                                                               On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Geovertex' 'nl' 'V';
Symbol read: '0.024072 ';                                         Token created: 'Float';                                       On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Geovertex' 'nl' 'V';
Shift token: 'Float';                                                                                                           On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Geovertex' 'nl' 'V' 'Float';
Symbol read: '0.007984 ';                                         Token created: 'Float';                                       On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Geovertex' 'nl' 'V' 'Float';
Shift token: 'Float';                                                                                                           On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Geovertex' 'nl' 'V' 'Float' 'Float';
Symbol read: '0.051516';                                          Token created: 'Float';                                       On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Geovertex' 'nl' 'V' 'Float' 'Float';
Shift token: 'Float';                                                                                                           On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Geovertex' 'nl' 'V' 'Float' 'Float' 'Float';
Symbol read: 'LF';                                                Token created: 'NewLine';                                     On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Geovertex' 'nl' 'V' 'Float' 'Float' 'Float';
Lookahead: 'NewLine';                                             Reduce: '<Geovertex> ::= V Float Float Float';                On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Geovertex' 'nl' 'Geovertex';
Shift token: 'NewLine';                                                                                                         On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Geovertex' 'nl' 'Geovertex' 'NewLine';
Symbol read: 'v ';                                                Token created: 'V';                                           On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Geovertex' 'nl' 'Geovertex' 'NewLine';
Lookahead: 'V';                                                   Reduce: '<nl> ::= ';                                          On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Geovertex' 'nl' 'Geovertex' 'NewLine' 'nl';
Lookahead: 'V';                                                   Reduce: '<nl> ::= NewLine <nl>';                              On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl';
Shift token: 'V';                                                                                                               On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'V';
Symbol read: '-0.000857 ';                                        Token created: 'Float';                                       On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'V';
...
Shift token: 'V';                                                                                                               On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'G...
Symbol read: '0.024072 ';                                         Token created: 'Float';                                       On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'G...
Shift token: 'Float';                                                                                                           On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'G...
Symbol read: '0.007984 ';                                         Token created: 'Float';                                       On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'G...
Shift token: 'Float';                                                                                                           On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'G...
Symbol read: '0.020334';                                          Token created: 'Float';                                       On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'G...
Shift token: 'Float';                                                                                                           On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'G...
Symbol read: 'LF';                                                Token created: 'NewLine';                                     On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'G...
Lookahead: 'NewLine';                                             Reduce: '<Geovertex> ::= V Float Float Float';                On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'G...
Shift token: 'NewLine';                                                                                                         On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'G...
Symbol read: 'v ';                                                Token created: 'V';                                           On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'G...
Lookahead: 'V';                                                   Reduce: '<nl> ::= ';                                          On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'G...
Lookahead: 'V';                                                   Reduce: '<nl> ::= NewLine <nl>';                              On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'G...
Shift token: 'V';                                                                                                               On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'G...
Symbol read: '0.000202 ';                                         Token created: 'Float';                                       On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'G...
Shift token: 'Float';                                                                                                           On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'G...
...
Shift token: 'V';                                                                                                               On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'G...
Symbol read: '0.008748 ';                                         Token created: 'Float';                                       On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'G...
Shift token: 'Float';                                                                                                           On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'G...
Symbol read: '0.015613 ';                                         Token created: 'Float';                                       On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'G...
Shift token: 'Float';                                                                                                           On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'G...
Symbol read: '0.051516';                                          Token created: 'Float';                                       On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'G...
Shift token: 'Float';                                                                                                           On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'G...
Symbol read: 'LF';                                                Token created: 'NewLine';                                     On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'G...
Lookahead: 'NewLine';                                             Reduce: '<Geovertex> ::= V Float Float Float';                On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'G...
Shift token: 'NewLine';                                                                                                         On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'G...
Symbol read: 'v ';                                                Token created: 'V';                                           On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'G...
Lookahead: 'V';                                                   Reduce: '<nl> ::= ';                                          On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'G...
Lookahead: 'V';                                                   Reduce: '<nl> ::= NewLine <nl>';                              On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'G...
Shift token: 'V';                                                                                                               On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'G...
Symbol read: '0.007089 ';                                         Token created: 'Float';                                       On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'G...
Shift token: 'Float';                                                                                                           On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'G...
Symbol read: '0.012602 ';                                         Token created: 'Float';                                       On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'G...
...
Symbol read: 'f ';                                                Token created: 'F';                                           On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'G...
Lookahead: 'F';                                                   Reduce: '<nl> ::= ';                                          On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'G...
Lookahead: 'F';                                                   Reduce: '<nl> ::= NewLine <nl>';                              On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'G...
Lookahead: 'F';                                                   Reduce: '<nl> ::= NewLine <nl>';                              On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'G...
Shift token: 'F';                                                                                                               On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'G...
Symbol read: '14/14 13/13 5/5';                                   Token created: 'Duplet';                                      On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'G...
Shift token: 'Duplet';                                                                                                          On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'G...
Symbol read: ' ';                                                 Token created: 'Whitespace';                                  On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'G...
Symbol read: 'LF'; Token created: 'NewLine';                                                                                    On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'G...
Lookahead: 'NewLine';                                             Reduce: '<Face> ::= F Duplet';                                On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'G...
Shift token: 'NewLine';                                                                                                         On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'G...
Symbol read: 'f ';                                                Token created: 'F';                                           On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'G...
Lookahead: 'F';                                                   Reduce: '<nl> ::= ';                                          On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'G...
Lookahead: 'F';                                                   Reduce: '<nl> ::= NewLine <nl>';                              On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'G...
Shift token: 'F';                                                                                                               On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'G...
Symbol read: '10/10 7/7 11/11';                                   Token created: 'Duplet';                                      On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'G...
Shift token: 'Duplet';                                                                                                          On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'G...
Symbol read: ' ';                                                 Token created: 'Whitespace';                                  On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'G...
...
Symbol read: '14/14 5/5 1/1 12/12';                               Token created: 'Duplet';                                      On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'G...
Shift token: 'Duplet';                                                                                                          On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'G...
Symbol read: ' ';                                                 Token created: 'Whitespace';                                  On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'G...
Symbol read: 'LF'; Token created: 'NewLine';                                                                                    On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'G...
Lookahead: 'NewLine';                                             Reduce: '<Face> ::= F Duplet';                                On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'G...
Shift token: 'NewLine';                                                                                                         On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'G...
Symbol read: 'LF';                                                Token created: 'NewLine';                                     On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'G...
Shift token: 'NewLine';                                                                                                         On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'G...
Symbol read: '';                                                  Token created: 'EOF';                                         On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'G...
Lookahead: 'EOF';                                                 Reduce: '<nl> ::= ';                                          On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'G...
Lookahead: 'EOF';                                                 Reduce: '<nl> ::= NewLine <nl>';                              On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'G...
Lookahead: 'EOF';                                                 Reduce: '<nl> ::= NewLine <nl>';                              On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'G...
Lookahead: 'EOF';                                                 Reduce: '<Program> ::= ';                                     On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'G...
Lookahead: 'EOF';                                                 Reduce: '<Program> ::= <Face> <nl> <Program>';                On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'G...
Lookahead: 'EOF';                                                 Reduce: '<Program> ::= <Face> <nl> <Program>';                On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'G...
...
Lookahead: 'EOF';                                                 Reduce: '<Program> ::= <TextureCoordinate> <nl> <Program>';   On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'G...
Lookahead: 'EOF';                                                 Reduce: '<Program> ::= <TextureCoordinate> <nl> <Program>';   On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'G...
Lookahead: 'EOF';                                                 Reduce: '<Program> ::= <TextureCoordinate> <nl> <Program>';   On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'G...
Lookahead: 'EOF';                                                 Reduce: '<Program> ::= <TextureCoordinate> <nl> <Program>';   On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'G...
Lookahead: 'EOF';                                                 Reduce: '<Program> ::= <TextureCoordinate> <nl> <Program>';   On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'G...
Lookahead: 'EOF';                                                 Reduce: '<Program> ::= <Geovertex> <nl> <Program>';           On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'G...
Lookahead: 'EOF';                                                 Reduce: '<Program> ::= <Geovertex> <nl> <Program>';           On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'G...
Lookahead: 'EOF';                                                 Reduce: '<Program> ::= <Geovertex> <nl> <Program>';           On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'G...
Lookahead: 'EOF';                                                 Reduce: '<Program> ::= <Geovertex> <nl> <Program>';           On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'G...
Lookahead: 'EOF';                                                 Reduce: '<Program> ::= <Geovertex> <nl> <Program>';           On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'Geovertex' 'nl' 'G...
...
Lookahead: 'EOF';                                                 Reduce: '<Program> ::= <Geovertex> <nl> <Program>';           On stack: 'nl' 'Comment' 'nl' 'Group' 'nl' 'Program';
Lookahead: 'EOF';                                                 Reduce: '<Program> ::= <Group> <nl> <Program>';               On stack: 'nl' 'Comment' 'nl' 'Program';
Lookahead: 'EOF';                                                 Reduce: '<Program> ::= <Comment> <nl> <Program>';             On stack: 'nl' 'Program';
Lookahead: 'EOF';                                                 Reduce: '<Start> ::= <nl> <Program>';                         On stack: 'Start';
Accepted!


Press any key to continue...
```

# Remarks, license
This project was created within the scope of the subject of Compilers and it's results may only be used for academic purposes only with the permission of the 
responsible teacher or those who involved in the development of this project  (including the developers of the original GOLD project).