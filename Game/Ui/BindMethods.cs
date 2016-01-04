using System;

namespace Game
{
    public partial class GameClass : Microsoft.Xna.Framework.Game
    {
        public void ExecuteInvocation(string invocation)
        {
            var pieces = invocation.Split('(', ')');

            string function_name = pieces[0];
            try
            {
                pieces = pieces[1].Split(',');
            }
            catch
            {
                pieces = null;
            }

            var function = this.GetType().GetMethod(function_name);
            var args = function.GetParameters();

            object[] typed_args = new object[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                string arg_str = pieces[i];
                var arg = args[i];
                object typed_arg = null;

                if (arg.ParameterType == typeof(int))
                {
                    typed_arg = int.Parse(arg_str);
                }
                else if (arg.ParameterType == typeof(bool))
                {
                    typed_arg = bool.Parse(arg_str);
                }
                else if (arg.ParameterType == typeof(float))
                {
                    typed_arg = float.Parse(arg_str);
                }
                else if (arg.ParameterType == typeof(string))
                {
                    typed_arg = arg_str;
                }

                typed_args[i] = typed_arg;
            }

            function.Invoke(this, typed_args);
        }
    }
}
