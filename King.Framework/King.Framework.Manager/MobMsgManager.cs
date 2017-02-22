namespace King.Framework.Manager
{
    using System;

    public class MobMsgManager
    {
        private static MobMsgManager manager;

        private MobMsgManager()
        {
        }

        public static MobMsgManager GetInstance()
        {
            if (manager == null)
            {
                manager = new MobMsgManager();
            }
            return manager;
        }

        public void SendMessger(string to)
        {
            string str = "123456";
            string str2 = "1380000000";
            string str3 = "111111";
            new MessageSender(str, str2, str3).TrySend(to);
        }
    }
}
