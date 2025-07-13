using Meme.Hub.Site.Models;

namespace Meme.Hub.Site.Services
{

    // A simple in-memory data store for demonstration purposes
    public class DataStore
    {
        public List<User> Users { get; set; } = new List<User>();
        public List<Post> Posts { get; set; } = new List<Post>();

        public DataStore()
        {
            // Seed some initial data for testing
            SeedData();
        }

        private void SeedData()
        {
            var user1 = new User
            {
                PrivyId = "privy_user_1", // Example Privy ID
                Username = "Alice_Wonder",
                Email = "alice@example.com",
                Bio = "Exploring the rabbit hole of Web3.",
                ProfileImage = "https://i.pravatar.cc/150?img=1"
            };
            var user2 = new User
            {
                PrivyId = "privy_user_2",
                Username = "Bob_Builder",
                Email = "bob@example.com",
                Bio = "Can we build it? Yes we can!",
                ProfileImage = "https://i.pravatar.cc/150?img=2"
            };
            var user3 = new User
            {
                PrivyId = "privy_user_3",
                Username = "Charlie_Coder",
                Email = "charlie@example.com",
                Bio = "Coding my way through the metaverse.",
                ProfileImage = "https://i.pravatar.cc/150?img=3"
            };

            Users.Add(user1);
            Users.Add(user2);
            Users.Add(user3);

            // Establish some follow relationships
            user1.Following.Add(user2._id);
            user2.Followers.Add(user1._id);

            user2.Following.Add(user3._id);
            user3.Followers.Add(user2._id);

            var post1 = new Post
            {
                UserId = user1._id,
                Content = "Just launched my first dApp on Solana! So excited! #web3 #solana",
                ImageUrl = "https://via.placeholder.com/400x200?text=dApp+Launch",
            };
            post1.Likes.Add(user2._id);
            post1.Comments.Add(new Comment { UserId = user2._id, Text = "Congrats Alice! Looks great!" });

            var post2 = new Post
            {
                UserId = user2._id,
                Content = "Working on a new smart contract for a decentralized game. Any tips on Rust development?",
                ImageUrl = "https://via.placeholder.com/400x200?text=Smart+Contract",
            };
            post2.Likes.Add(user1._id);
            post2.Comments.Add(new Comment { UserId = user1._id, Text = "Rust is amazing! Check out Anchor framework for Solana." });

            Posts.Add(post1);
            Posts.Add(post2);
        }
    }
}
