using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace lab6.Models
{
    public class ForumContext : DbContext
    {
        public DbSet<UserModel> Users { get;set; }
        public DbSet<ForumCategoryModel> ForumCategories { get; set; }
        public DbSet<TopicModel> Topics { get; set; }
        public DbSet<ReplyModel> Replies { get; set; }
        public DbSet<RoleModel> Roles { get; set; }
        public DbSet<PictureModel> Pictures { get; set; }
        public DbSet<FolderModel> Folders { get; set; }
        public DbSet<FileModel> Files { get; set; }


        public ForumContext(DbContextOptions<ForumContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RoleModel>().HasData(
                new RoleModel { Id = 1, Name = "admin" },
                new RoleModel { Id = 2, Name = "user" } );

            modelBuilder.Entity<UserModel>().HasData(
                new UserModel
                {
                    Id = 1,
                    Email = "admin@mail.ru",
                    Password = "admin",
                    RoleId = 1
                },
                new UserModel
                {
                    Id = 2,
                    Email = "user@mail.ru",
                    Password = "user",
                    RoleId = 2
                });

            modelBuilder.Entity<ForumCategoryModel>().HasData(
                new ForumCategoryModel
                {
                    Id = 1,
                    Name = "Rock music",
                    Description = "About rock music and famous rock artist."
                },
                new ForumCategoryModel 
                {
                    Id = 2,
                    Name = "Business",
                    Description = "About new trends in business"
                },
                new ForumCategoryModel
                {
                    Id = 3,
                    Name = "IT",
                    Description = "Some news in IT sphere"
                });

            modelBuilder.Entity<PictureModel>().HasData(
                new PictureModel
                {
                    Id = 1,
                    Name = "etf",
                    Image = ReadFile("wwwroot/img/etf.jpg"),
                    ReplyModelId = 2
                },
                new PictureModel
                {
                    Id = 2,
                    Name = "Mick",
                    Image = ReadFile("wwwroot/img/Mick.jpg"),
                    ReplyModelId = 1
                },
                new PictureModel
                {
                    Id = 3,
                    Name = "Mick Jagger",
                    Image = ReadFile("wwwroot/img/Mick_Jagger02.jpg"),
                    TopicModelId = 1
                });

            modelBuilder.Entity<TopicModel>().HasData(
                new TopicModel
                {
                    Id = 1,
                    Title = "Mick Jagger",
                    Description = "Jagger plays a part of himself every time he goes on stage, providing performances of a lifetime to his audience. Back when Jagger and The Rolling Stones started playing for bigger audiences, they went from wearing only what they had to being given stage clothes. The famous clothes he wore in the 60s and 70s are intrinsically connected with his music. In his own words, a 15,000-seat arena requires both colour and silhouette and clothing items that stand out.",
                    DateCreated = DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss"),
                    DateEdited = DateTime.Now,
                    ReplyCount = 1,
                    ForumId = 1,
                    AuthorId = 1,
                    LastReplyId = 2,
                    PictureCount = 2
                },
                new TopicModel
                {
                    Id = 2,
                    Title = "How do I invest in Crypto ETFs/ETNs?",
                    Description = "Cryptocurrencies are independent of conventional, governmental currency systems such as the Euro or the Dollar. Popular cryptocurrencies are, for example, Bitcoin, Ether, Bitcoin Cash and Ripple. For cryptocurrencies, the right to ownership is controlled through the possession of computer-generated keys. Payments are cryptographically legitimised and processed via a network of computers with equal rights – without the need for a bank. The administration is based on a decentralised, synchronised accounting system, the so-called blockchain.",
                    DateCreated = DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss"),
                    DateEdited = DateTime.Now,
                    ReplyCount = 2,
                    ForumId = 2,
                    AuthorId = 2,
                    LastReplyId = 1,
                    PictureCount = 1
                });

            modelBuilder.Entity<ReplyModel>().HasData(
                new ReplyModel
                {
                    Id = 1,
                    Text = "My love <3 ",
                    DateCreated = DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss"),
                    DateEdited = DateTime.Now,
                    TopicId = 1,
                    AuthorId = 2,
                    PictureCount = 3
                },
                new ReplyModel
                {
                    Id = 2,
                    Text = "Cool man",
                    DateCreated = DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss"),
                    DateEdited = DateTime.Now,
                    TopicId = 2,
                    AuthorId = 1,
                    PictureCount = 0
                });           

            modelBuilder.Entity<FolderModel>().HasData(
                new FolderModel
                {
                    Id = 1,
                    Name = "Root"
                });
        }


       
        public ReplyModel DeleteAllPicturesInReply(ReplyModel reply)
        {
            if (reply.PictureCount != 0)
            {
                foreach (PictureModel pic in reply.Pictures.ToList())
                {
                    reply.Pictures.Remove(pic);
                    this.Pictures.Remove(pic);
                }
            }
            return reply;
        }
        public TopicModel DeleteAllPicturesInTopic(TopicModel topic)
        {
            if (topic.PictureCount != 0)
            {
                foreach (PictureModel pic in topic.Pictures.ToList())
                {
                    topic.Pictures.Remove(pic);
                    this.Pictures.Remove(pic);
                }
            }
            return topic;
        }
        public TopicModel DeleteAllRepliesInTopic(TopicModel topic)
        {
            if(topic.ReplyCount != 0)
            {
                foreach (ReplyModel reply in topic.Replies.ToList())
                {
                    DeleteReply(reply, topic);
                }
            }
            return topic;
        }
        public ForumCategoryModel DeleteAllTopicsInCategory(ForumCategoryModel forumCategory)
        {
            if (forumCategory.Topics.Any())
            {
                foreach (TopicModel topic in forumCategory.Topics.ToList())
                {
                    DeleteTopic(topic);
                }
            }
            return forumCategory;
        }

        public void DeleteForumCategory(ForumCategoryModel forumCategory)
        {
            forumCategory = DeleteAllTopicsInCategory(forumCategory);
            this.ForumCategories.Remove(forumCategory);
        }
        public void DeleteReply(ReplyModel reply, TopicModel topic)
        {
            reply = DeleteAllPicturesInReply(reply);
            topic.Replies.Remove(reply);
            topic.ReplyCount--;
            this.Replies.Remove(reply);
        }
        public void DeleteTopic(TopicModel topic)
        {
            topic = DeleteAllRepliesInTopic(topic);
            topic = DeleteAllPicturesInTopic(topic);
            this.Topics.Remove(topic);
        }
        public void DeleteFile(FileModel file)
        {
           FolderModel folder = this.Folders.FirstOrDefault(x => x.Id == file.FolderId);
           folder.Files.Remove(file);
           this.Files.Remove(file);
        }
        public void DeleteFolder(FolderModel folder)
        {
            FolderModel ParentFolder = this.Folders.FirstOrDefault(x => x.Id == folder.ParentFolderId);
            ParentFolder.Folders.Remove(folder);
            this.Folders.Remove(folder);
        }

        public void DeleteFolderDirectory(FolderModel folder)
        {
            if (folder.Files.Any())
            {
                foreach(FileModel file in folder.Files.ToList())
                {
                    DeleteFile(file);
                }
            }

            if (folder.Folders.Any())
            {
                foreach (FolderModel fld in folder.Folders.ToList())
                {
                    FolderModel fld_temp = this.Folders
                        .Include(x => x.Folders)
                        .Include(x => x.Files)
                        .FirstOrDefault(x => x.Id == fld.Id);

                    if (fld.Folders.Any())
                    {
                        DeleteFolderDirectory(fld);
                    }
                    DeleteFolder(fld);
                } 
            }
            DeleteFolder(folder);
        }



        // method for seeding img data
        public byte[] ReadFile(string path)
        {
            FileInfo fInfo = new(path);
            long numBytes = fInfo.Length;

            FileStream fStream = new(path, FileMode.Open, FileAccess.Read);

            BinaryReader br = new(fStream);

            byte[] data = br.ReadBytes((int)numBytes);
            return data;
        }
    }
}