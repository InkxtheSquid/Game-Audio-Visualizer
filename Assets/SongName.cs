using System.Collections;
using System.Collections.Generic;
using System;
public class SongName
{
    private string songname;

    SongName(string s){
        songname=s;
    }

    SongName(){}
    
    public void setSongName(string s){
        songname=s;
    }

    public string getSongName(){
        return songname;
    }
}
