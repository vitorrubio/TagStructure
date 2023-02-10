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

        public Tags(Tags? tags) : this(tags?.GetTags())
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
            this.AddTags(t?.ToArray());
        }

        public void AddTags(Tags? t)
        {
            this.AddTags(t?.GetTags());
        }



        public void AddTags(params string[]? t)
        {
            if (t != null && t.Length > 0)
            {
                var tagsToAdd = t.Where(x => !string.IsNullOrWhiteSpace(x)).SelectMany(x => x!.Split(",")).Select(x => x.Trim().ToLower());
                _taglist.UnionWith(tagsToAdd);
            }
        }


        public void RemoveTags(IEnumerable<string>? t)
        {
            this.RemoveTags(t?.ToArray());
        }


        public void RemoveTags(Tags? tags)
        {
            this.RemoveTags(tags?.GetTags());
        }



        public void RemoveTags(params string[]? t)
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
            if (object.ReferenceEquals(this, obj))
            {
                return true;
            }

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
    }
}