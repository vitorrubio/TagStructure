namespace SharpTags
{
    public struct Tags
    {
        private readonly HashSet<string> _taglist = new HashSet<string>();

        public Tags()
        {
        }

        public Tags(IEnumerable<string>? tags) : this(tags?.ToArray())
        {

        }

        public Tags(params string[]? t)
        {
            if (t != null)
            {
                this._taglist.UnionWith(t.Where(x => !string.IsNullOrWhiteSpace(x)).SelectMany(x => x!.Split(",")).Select(x => x.Trim().ToLower()));
            }
        }

        public override string ToString()
        {
            return string.Join(",", _taglist.Select(x => x.Trim().ToLower()).OrderBy(x => x).Distinct());
        }


        public void AddTags(IEnumerable<string>? t)
        {
            this.Add(t?.ToArray());
        }





        public void Add(params string[]? t)
        {
            if (t != null && t.Length > 0)
            {
                var tagsToAdd = t.Where(x => !string.IsNullOrWhiteSpace(x)).SelectMany(x => x!.Split(",")).Select(x => x.Trim().ToLower());
                _taglist.UnionWith(tagsToAdd);
            }
        }


        public void Remove(IEnumerable<string>? t)
        {
            this.Remove(t?.ToArray());
        }






        public void Remove(params string[]? t)
        {
            if (t != null && t.Length > 0)
            {
                var tagsToRemove = t.Where(x => !string.IsNullOrWhiteSpace(x)).SelectMany(x => x!.Split(",")).Select(x => x.Trim().ToLower());
                _taglist.ExceptWith(tagsToRemove);

            }

        }


        public string[] GetTags()
        {
            return this._taglist.Select(x => x.Trim().ToLower()).OrderBy(x => x).Distinct().ToArray();
        }


        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        public override bool Equals(object? obj)
        {

            if (obj == null)
            {
                return false;
            }

            if ((!(obj is Tags)) && (!(obj is string)))
            {
                return false;
            }

            return this.ToString().Equals(obj.ToString());
        }



        public static bool operator ==(Tags esquerda, Tags direita)
        {
            return esquerda.Equals(direita);
        }

        public static bool operator !=(Tags esquerda, Tags direita) => !(esquerda == direita);


        public static implicit operator string(Tags t) => t.ToString();

        public static implicit operator Tags(string s) => new Tags(s);
    }
}