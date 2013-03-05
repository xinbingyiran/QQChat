// ConsoleApplication1.cpp : 定义控制台应用程序的入口点。
//

#include "stdafx.h"
#include<stdio.h>
void printchs(char& ch)
{
	printf("%s",**(char***)&ch);//如何通过ch，输出"this is a test string"
}

int main(int a,char* b[],char** c)
{
	char** chs = new char*[1];
	chs[0] = "this is a test string";
	char& ch = (char&)chs;
	printchs(ch);
	delete chs;
}



