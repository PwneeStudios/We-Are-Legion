using System;
using System.Collections.Generic;

using FragSharpFramework;

namespace Game
{
    public partial class World : SimShader
    {
        public void Message_DragonLordUnderAttack() { AddUserMessage(50, "Your Dragon Lord is under attack!"); }
        public void Message_InsufficientGold() { AddUserMessage("Insufficient gold."); }
        public void Message_InsufficientJade() { AddUserMessage("Insufficient jade."); }
        public void Message_NoDragonLordMagic() { AddUserMessage(50, "Your Dragon Lord is dead and cannot cast magic."); }
        public void Message_NoDragonLordConstruction() { AddUserMessage(50, "Your Dragon Lord is dead and cannot order new constructions."); }
        public void Message_CanNotPlaceHere() { AddUserMessage("Can't place here."); }
        
        public void Message_PlayerDefeated(int PlayerNum)
        {
            try
            {
                var player = PlayerInfo[PlayerNum];

                AddUserMessage(110, "{0} has been defeated!", player.Name);
            }
            catch (Exception e)
            { 
                
            }
        }
    }

    public class UserMessageList
    {
        List<UserMessage> UserMessages = new List<UserMessage>();

        public void Add(UserMessage UserMessage)
        {
            UserMessages.Add(UserMessage);
        }

        public void Draw()
        {
            vec2 pos = GameClass.Screen * new vec2(.5f, .8f) - UserMessage.Spacing * UserMessages.Count;

            bool TextStartedAlready = Render.TextStarted;
            if (!TextStartedAlready) Render.StartText();

            foreach (var message in UserMessages)
            {
                message.Draw(pos);
                pos += UserMessage.Spacing;
            }

            if (!TextStartedAlready) Render.EndText();
        }

        public void Update()
        {
            foreach (var UserMessage in UserMessages) UserMessage.Update();
            UserMessages.RemoveAll(UserMessage => UserMessage.Dead);
        }
    }

    public class UserMessage
    {
        public const int Duration = 43;
        public const float alpha_fade = -1.0f;
        public static readonly vec2 Spacing = new vec2(0, 38);

        public string Message = string.Empty;

        float alpha = 1;
        int age = 0;

        World world;

        public UserMessage(World world, string message, int extra_life = 0)
        {
            this.world = world;
            this.Message = message;

            age -= extra_life;
        }

        public bool Dead { get { return alpha <= 0; } }

        public void Draw(vec2 pos)
        {
            Render.DrawText(Message, pos, .785f, align : Alignment.Center, clr : new color(1f, 1f, 1f, alpha));
        }

        public void Update()
        {
            age++;

            if (age > Duration)
            { 
                alpha += (float)GameClass.Time.ElapsedGameTime.TotalSeconds * alpha_fade;
            }
        }
    }
}
