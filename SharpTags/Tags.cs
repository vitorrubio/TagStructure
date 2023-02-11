using System;
using System.Collections.Immutable;

namespace SharpTags
{
    /// <summary>
    /// a struct (Value Type) representing a collection of unique tags
    /// </summary>
    public struct Tags : IEquatable<Tags>
    {
        #region private fields
        /// <summary>
        /// ImmutableHashSet with the tags
        /// </summary>
        private readonly ImmutableHashSet<string> _taglist;

        #endregion



        #region constructors

        /// <summary>
        /// default blank constructor
        /// </summary>
        public Tags() : this(null)
        {
        }

        /// <summary>
        /// create tags from a IEnumerable of strings
        /// </summary>
        /// <param name="t"></param>
        public Tags(IEnumerable<string>? t) : this(t?.ToArray())
        {
        }

        /// <summary>
        /// create tags from an array of strings
        /// </summary>
        /// <param name="t"></param>
        public Tags(params string[]? t)
        {
            if (t != null && t.Count() > 0)
            {
                var tagsToAdd = t.Where(x => !string.IsNullOrWhiteSpace(x)).SelectMany(x => x!.Split(",")).Select(x => x.Trim().ToLower()).ToArray();
                _taglist = ImmutableHashSet.Create<string>(tagsToAdd);
            }
            else
            {
                _taglist = ImmutableHashSet.Create<string>();
            }
        }
        #endregion



        #region implementation of overrides of ToString, GetHashCode and Equals

        /// <summary>
        /// convert current tags to a lowercase comma separated string, ordered and trimmed, of unique tags
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Join(",", _taglist.Select(x => x.Trim().ToLower()).OrderBy(x => x).Distinct());
        }

        /// <summary>
        /// overrides object GetHashCode using string's GetHashCode implementation
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        /// <summary>
        /// override equals consider strings too
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
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

        /// <summary>
        /// IEquatable implementation of Equals
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool Equals(Tags other)
        {
            return this.ToString().Equals(other.ToString());
        }

        #endregion



        #region public dynamic methods (Add, Remove and GetTags)

        /// <summary>
        /// adds an IEnumerable of tags and returns resulting Tags
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public Tags Add(IEnumerable<string>? t)
        {
            return this.Add(t?.ToArray());
        }

        /// <summary>
        /// adds an array of tags and returns resulting Tags
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public Tags Add(params string[]? t)
        {
            if (t != null && t.Length > 0)
            {
                var tagsToAdd = t.Where(x => !string.IsNullOrWhiteSpace(x)).SelectMany(x => x!.Split(",")).Select(x => x.Trim().ToLower());
                return new Tags(_taglist.Union(tagsToAdd));
            }

            return new Tags(_taglist);
        }

        /// <summary>
        /// remove an IEnumerable of tags and returns resulting Tags
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public Tags Remove(IEnumerable<string>? t)
        {
            return this.Remove(t?.ToArray());
        }

        /// <summary>
        /// remove a tag or array of tags and returns resulting Tags
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public Tags Remove(params string[]? t)
        {
            if (t != null && t.Length > 0)
            {
                var tagsToRemove = t.Where(x => !string.IsNullOrWhiteSpace(x)).SelectMany(x => x!.Split(",")).Select(x => x.Trim().ToLower());
                return new Tags(_taglist.Except(tagsToRemove));
            }
            return new Tags(_taglist);
        }

        /// <summary>
        /// get tags as string[]
        /// </summary>
        /// <returns></returns>
        public string[] GetTags()
        {
            return this._taglist.Select(x => x.Trim().ToLower()).OrderBy(x => x).Distinct().ToArray();
        }



        #endregion



        #region static methos operator overloading

        /// <summary>
        /// implicit Tags to string conversion
        /// </summary>
        /// <param name="t"></param>
        public static implicit operator string(Tags t) => t.ToString();

        /// <summary>
        /// implicit string to Tags conversion
        /// </summary>
        /// <param name="s"></param>
        public static implicit operator Tags(string s) => new Tags(s);

        /// <summary>
        /// equality == operator overloading
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(Tags left, Tags right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// inequality != operator overloading
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(Tags left, Tags right) => !(left == right);

        #endregion
    }
}