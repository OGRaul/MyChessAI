using System;
using Board;
using Moves;
using Util;


namespace Engine
{
    public static class AI
    {
        //TODO: change the ai here to the real AI
        public static RandomAI engine = new RandomAI();
        
        public static void makeEngineMove()
        {
            engine.playMove();
        }
    }   
}