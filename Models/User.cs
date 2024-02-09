using Newtonsoft.Json;

namespace RAI_2.Models
{
    public class User
    {
        public string Username;
        public DateTime CreatedDate = DateTime.Now;

        [JsonIgnore]
        private List<string> Friends;


        public User(string Username)
        {
            this.Username = Username;
            Friends = new List<string>();
        }

        public bool AddFriend(List<User> users, User friend) {
            if (friend == null || Friends.Contains(friend.Username) || !users.Contains(friend) || friend.Equals(this)) return false;
            Friends.Add(friend.Username);
            return true;
        }
        public bool AddFriend(List<User> users, string friend)
        {
            if (Friends.Contains(friend) || users.Find(u => u.Username == friend) == null || friend.Equals(this.Username)) return false;
            Friends.Add(friend);
            return true;
        }
        public void AddFriends(IEnumerable<User> friends, List<User> users) { foreach (User friend in friends) { AddFriend(users, friend); } }

        public bool RemoveFriend(string friend) { return Friends.Remove(friend); }
        public bool ImportFriends(List<User> users, string json)
        {
            if (json == null) return false;

            List<string> temp = Friends;
            Friends = new List<string>();

            foreach (string friend in JsonConvert.DeserializeObject<List<string>>(json))
            {
                AddFriend(users, friend);
            }

            if (Friends.Count == 0)
            {
                Friends = temp;
                return false;
            }

            return true;
        }

        public List<string> GetFriends() { return Friends; }

        public string GetFriendsAsJSON()
        {
            return JsonConvert.SerializeObject(Friends);
        }

    }
}
