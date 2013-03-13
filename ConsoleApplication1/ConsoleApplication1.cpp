/* 
 * combinationForNumber.cpp 
 * 
 *  Created on: 2012-10-19 
 *      Author: happier 
 */  
#include "stdafx.h"
#include <iostream>   
#include <string.h>   
#include <cstdio>   
#include <cstdlib>   
using namespace std;  
  
#define MAX_VALUE   8   
int thenext[MAX_VALUE] = { 0 };    //可以理解成一个链表，thenext[i]表示i数后面跟着的数字   
  
/* 
 * 递归进行求解 
 * @nSum 目标和 
 * @pData 保存已经存在的数字 
 * @nDepth 记录当前已经保存数据的个数 
 */  
void SegNum(int nSum, int* pData, int nDepth)  
{  
    if (nSum < 0)  
        return;  
  
    //如果已经符合要求，开始输出   
    if (nSum == 0)  
    {  
		cout<<MAX_VALUE<<" = ";
		int j;
        for (j = 0; j < nDepth - 1; j++)  
            cout << pData[j] << " + ";  
		cout<<pData[j];
        cout << endl;  
  
        return;  
    }  
  
    //这里有一个小trick，如果要求呈现递增，采用第一种赋值方式   
    //如果可以是重复的，即非递增方式，采用第二种赋值   
    int i = (nDepth == 0 ? thenext[0] : pData[nDepth - 1]);  
    //int i = thenext[0];   
    for (; i <= nSum;)  
    {  
        pData[nDepth++] = i;  
        SegNum(nSum - i, pData, nDepth);  
        nDepth--;   //递归完成后，将原来的数据弹出来，并且去链表中的下一个数字   
  
        i = thenext[i];  
    }  
  
    return ;  
}  
  
void ShowResult(int array[], int nLen)  
{  
    thenext[0] = array[0];  
    int i = 0;  
    for (; i < nLen - 1; i++)  
        thenext[array[i]] = array[i + 1];  //下一个可选数字大小   
    thenext[array[i]] = array[i] + 1;  //thenext[MAX_VALUE]大于MAX_VALUE，一个小trick，避免了很多比较   
  
    int* pData = new int[MAX_VALUE];  
    SegNum(MAX_VALUE, pData, 0);  
    delete[] pData;  
}  
  
int main()  
{  
    int* array = new int[MAX_VALUE];  
    for (int i = 0; i < MAX_VALUE; i++)  
    {  
        array[i] = i + 1;  
    }  
    //找零钱测试   
    ShowResult(array, MAX_VALUE);  
  
    //system("pause");   
    return 0;  
} 