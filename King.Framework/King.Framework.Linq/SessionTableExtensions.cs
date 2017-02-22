namespace King.Framework.Linq
{
    using System;
    using System.Runtime.CompilerServices;

    public static class SessionTableExtensions
    {
        public static void DeleteOnSubmit(this ISessionTable table, object instance)
        {
            table.SetSubmitAction(instance, SubmitAction.Delete);
        }

        public static void DeleteOnSubmit<T>(this ISessionTable<T> table, T instance)
        {
            table.SetSubmitAction(instance, SubmitAction.Delete);
        }

        public static void InsertOnSubmit(this ISessionTable table, object instance)
        {
            table.SetSubmitAction(instance, SubmitAction.Insert);
        }

        public static void InsertOnSubmit<T>(this ISessionTable<T> table, T instance)
        {
            table.SetSubmitAction(instance, SubmitAction.Insert);
        }

        public static void InsertOrUpdateOnSubmit(this ISessionTable table, object instance)
        {
            table.SetSubmitAction(instance, SubmitAction.InsertOrUpdate);
        }

        public static void InsertOrUpdateOnSubmit<T>(this ISessionTable<T> table, T instance)
        {
            table.SetSubmitAction(instance, SubmitAction.InsertOrUpdate);
        }

        public static void UpdateOnSubmit(this ISessionTable table, object instance)
        {
            table.SetSubmitAction(instance, SubmitAction.Update);
        }

        public static void UpdateOnSubmit<T>(this ISessionTable<T> table, T instance)
        {
            table.SetSubmitAction(instance, SubmitAction.Update);
        }
    }
}
