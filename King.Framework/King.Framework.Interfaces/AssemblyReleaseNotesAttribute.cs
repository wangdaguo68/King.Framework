namespace King.Framework.Interfaces
{
    using System;

    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple=false, Inherited=false)]
    public class AssemblyReleaseNotesAttribute : Attribute
    {
        private readonly string _notes;

        public AssemblyReleaseNotesAttribute(string notes)
        {
            this._notes = notes;
        }

        public string ReleaseNotes
        {
            get
            {
                return this._notes;
            }
        }
    }
}

