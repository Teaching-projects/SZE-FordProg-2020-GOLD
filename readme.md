# What this project is all about?
The aim of this project is to generally get to know how parser generators work. Throughout my learning path I'm using a long forgotten but much as good LALR type parsing system called Grammar Oriented Language Developer aka GOLD (http://www.goldparser.org/grammars/index.htm).

# What are the aims of this project?
Besides the upper mentioned goals, my aim is to create a cross-platform (Windows/Linux/OSX) parser that can parse and debug grammars in "real-time" created by GOLD Builder.

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
  - Start the parser app from a Prompt/Terminal with [-f] switch followed by the absolute path of the grammar file.
  - Type in the syntax that is waiting to be parsed.
  - Read the response and repeat as many times as you want.
  - Type "exit" for quit from the app.

# Example
### On Windows:
```
DriveLetter:\path-to-file\GOLD.Parser.exe -f "DriveLetter:\path-to-file\grammar.egt"

Type your syntax here: a+b

        Symbol read: a;         Token created: Identifier;      On stack:
        Shift token: Identifier;                                On stack: 'Identifier'
        Symbol read: +;         Token created: +;               On stack: 'Identifier'
        Lookahead: '+';         Reduce: 'Value';                On stack: 'Value'
        Lookahead: '+';         Reduce: 'Negate Exp';           On stack: 'Negate Exp'
        Lookahead: '+';         Reduce: 'Mult Exp';             On stack: 'Mult Exp'
        Lookahead: '+';         Reduce: 'Expression';           On stack: 'Expression'
        Shift token: +;                                         On stack: 'Expression' '+'
        Symbol read: b;         Token created: Identifier;      On stack: 'Expression' '+'
        Shift token: Identifier;                                On stack: 'Expression' '+' 'Identifier'
        Symbol read: ;          Token created: EOF;             On stack: 'Expression' '+' 'Identifier'
        Lookahead: 'EOF';       Reduce: 'Value';                On stack: 'Expression' '+' 'Value'
        Lookahead: 'EOF';       Reduce: 'Negate Exp';           On stack: 'Expression' '+' 'Negate Exp'
        Lookahead: 'EOF';       Reduce: 'Mult Exp';             On stack: 'Expression' '+' 'Mult Exp'
        Lookahead: 'EOF';       Reduce: 'Expression';           On stack: 'Expression'
        Lookahead: 'EOF';       Reduce: 'Program';              On stack: 'Program'
        Accepted!


Type your syntax here: exit
```
### On Linux:
```
/path-to-file/GOLD.Parser -f "/path-to-file/grammar.egt"
Type your syntax here: a+b

        Symbol read: a;         Token created: Identifier;      On stack:
        Shift token: Identifier;                                On stack: 'Identifier'
        Symbol read: +;         Token created: +;               On stack: 'Identifier'
        Lookahead: '+';         Reduce: 'Value';                On stack: 'Value'
        Lookahead: '+';         Reduce: 'Negate Exp';           On stack: 'Negate Exp'
        Lookahead: '+';         Reduce: 'Mult Exp';             On stack: 'Mult Exp'
        Lookahead: '+';         Reduce: 'Expression';           On stack: 'Expression'
        Shift token: +;                                         On stack: 'Expression' '+'
        Symbol read: b;         Token created: Identifier;      On stack: 'Expression' '+'
        Shift token: Identifier;                                On stack: 'Expression' '+' 'Identifier'
        Symbol read: ;          Token created: EOF;             On stack: 'Expression' '+' 'Identifier'
        Lookahead: 'EOF';       Reduce: 'Value';                On stack: 'Expression' '+' 'Value'
        Lookahead: 'EOF';       Reduce: 'Negate Exp';           On stack: 'Expression' '+' 'Negate Exp'
        Lookahead: 'EOF';       Reduce: 'Mult Exp';             On stack: 'Expression' '+' 'Mult Exp'
        Lookahead: 'EOF';       Reduce: 'Expression';           On stack: 'Expression'
        Lookahead: 'EOF';       Reduce: 'Program';              On stack: 'Program'
        Accepted!


Type your syntax here: exit
```

# Remarks, licence
This project was created within the scope of the subject of Compilers and it's results may only be used for academic purposes only with the permission of the responsible teacher or those who involved in the development of this project  (including the developers of the original GOLD project).
