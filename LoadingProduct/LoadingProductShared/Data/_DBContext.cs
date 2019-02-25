using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace LoadingProductShared.Data
{
    public class AppDBContext : IdentityDbContext<AppUser, AppRole, int>
    {
        public virtual DbSet<AppSetting> AppSettings { get; set; }

        public virtual DbSet<MediaAlbum> MediaAlbums { get; set; }
        public virtual DbSet<MediaFile> MediaFiles { get; set; }

        public virtual DbSet<Taxonomy> Taxonomies { get; set; }
        public virtual DbSet<Service> Services { get; set; }

        public virtual DbSet<UserThread> UserThreads { get; set; }
        public virtual DbSet<UserMessage> UserMessages { get; set; }
        

        public AppDBContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AccountLog>(b =>
            {
                b.HasIndex(e => e.Action);
                b.HasIndex(e => e.LogTime);
            });

            builder.Entity<Taxonomy>(b =>
            {
                b.HasIndex(e => new { e.Name, e.Type });
            });
        }

        //public virtual IQueryable<BlogPost> CurrentPosts
        //{
        //    get { return BlogPosts.Where(x => x.Status >= PostStatus.Normal && x.PublishTime <= DateTime.Now); }
        //}

        //public virtual IQueryable<BlogPost> NormalPosts
        //{
        //    get { return BlogPosts.Where(x => x.Status == PostStatus.Normal && x.PublishTime <= DateTime.Now); }
        //}

        //public virtual IQueryable<BlogPost> SpecialPosts
        //{
        //    get { return BlogPosts.Where(x => x.Status == PostStatus.Special && x.PublishTime <= DateTime.Now); }
        //}

        //public virtual IQueryable<Taxonomy> PostCats
        //{
        //    get { return Taxonomies.Where(x => x.Type == TaxoType.PostCat); }
        //}

        //public virtual IQueryable<Taxonomy> PostTags
        //{
        //    get { return Taxonomies.Where(x => x.Type == TaxoType.PostTag); }
        //}

        //public virtual IQueryable<Taxonomy> ItemCats
        //{
        //    get { return Taxonomies.Where(x => x.Type == TaxoType.ItemCat); }
        //}

        //public virtual IQueryable<Taxonomy> ItemTags
        //{
        //    get { return Taxonomies.Where(x => x.Type == TaxoType.ItemTag); }
        //}

        //public virtual IQueryable<Taxonomy> Exports
        //{
        //    get { return Taxonomies.Where(x => x.Type == TaxoType.Export); }
        //}
        //public virtual IQueryable<Taxonomy> Sizes
        //{
        //    get { return Taxonomies.Where(x => x.Type == TaxoType.Size); }
        //}
    }
}

