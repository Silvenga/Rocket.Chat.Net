namespace Rocket.Chat.Net.Models
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    using Rocket.Chat.Net.JsonConverters;

    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class Attachment
    {
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "title_link")]
        public string TitleLink { get; set; }

        [JsonProperty(PropertyName = "title_link_download")]
        public bool? TitleLinkDownloadable { get; set; }

        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }

        [JsonProperty(PropertyName = "image_url")]
        public string ImageUrl { get; set; }

        [JsonProperty(PropertyName = "author_name")]
        public string AuthorName { get; set; }

        [JsonProperty(PropertyName = "author_icon")]
        public string AuthorIcon { get; set; }

        [JsonProperty(PropertyName = "thumb_url")]
        public string ThumbUrl { get; set; }

        [JsonProperty(PropertyName = "actions")]
        public IEnumerable<Action> Actions { get; set; }

        /// <summary>
        /// e.g. #FF0000
        /// </summary>
        [JsonProperty(PropertyName = "color")]
        public string Color { get; set; }

        // [JsonProperty(PropertyName = "ts"), JsonConverter(typeof(MeteorDateConverter))]
        [JsonProperty(PropertyName = "ts")]
        public DateTime? Timestamp { get; set; }

        protected bool Equals(Attachment other)
        {
            return string.Equals(Title, other.Title) && string.Equals(TitleLink, other.TitleLink) && TitleLinkDownloadable == other.TitleLinkDownloadable
                   && string.Equals(Text, other.Text) && string.Equals(ImageUrl, other.ImageUrl) && string.Equals(AuthorName, other.AuthorName)
                   && string.Equals(AuthorIcon, other.AuthorIcon) && string.Equals(ThumbUrl, other.ThumbUrl) && string.Equals(Color, other.Color) && string.Equals(Actions, other.Actions)
                   && Timestamp.Equals(other.Timestamp);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != this.GetType())
                return false;
            return Equals((Attachment) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Title != null ? Title.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (TitleLink != null ? TitleLink.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ TitleLinkDownloadable.GetHashCode();
                hashCode = (hashCode * 397) ^ (Text != null ? Text.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (ImageUrl != null ? ImageUrl.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (AuthorName != null ? AuthorName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (AuthorIcon != null ? AuthorIcon.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (ThumbUrl != null ? ThumbUrl.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Color != null ? Color.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Timestamp.GetHashCode();
                return hashCode;
            }
        }
    }
}