#pragma once
#include <fstream>
extern std::fstream LogCOM;
extern LONG Seq;
#define SEQ LONG __XXCSeq=InterlockedIncrement(&Seq)
#define LOG(x,y) LogCOM<<__XXCSeq<<":"<<(x)<<(y)<<"\n"
