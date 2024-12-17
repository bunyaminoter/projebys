using projebys.Models;
using Microsoft.EntityFrameworkCore;

namespace projebys.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Users> Users { get; set; }
        public DbSet<Advisors> Advisors { get; set; }
        public DbSet<Students> Students { get; set; }
        public DbSet<Courses> Courses { get; set; }
        public DbSet<Transcripts> Transcripts { get; set; }
        public DbSet<StudentCourseSelections> StudentCourseSelections { get; set; }
        public DbSet<ClassCourseMappings> ClassCourseMappings { get; set; }  // ClassCourseMappings tablosunu ekledim
        public DbSet<Classes> Classes { get; set; } // Classes tablosunu ekledim

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Advisors>().ToTable("Advisors");

            // Kullanıcılar tablosu için birincil anahtar
            modelBuilder.Entity<Users>()
                .HasOne(u => u.Advisor)               // Kullanıcı bir danışmana sahip olabilir
                .WithMany()                            // Bir danışmanın birden fazla kullanıcısı olabilir
                .HasForeignKey(u => u.RelatedID);    // Users tablosundaki RelatedID, AdvisorID'ye bağlanacak

            // Danışmanlar (Advisors) tablosu için birincil anahtar
            modelBuilder.Entity<Advisors>().HasKey(a => a.AdvisorID);

            // Öğrenciler (Students) tablosu için birincil anahtar
            modelBuilder.Entity<Students>().HasKey(s => s.StudentID);

            // Students -> Advisors ilişkisi
            modelBuilder.Entity<Students>()
                .HasOne<Advisors>()                   // Her öğrenci bir danışmana sahip olabilir
                .WithMany()                           // Bir danışmanın birden fazla öğrencisi olabilir
                .HasForeignKey(s => s.AdvisorID);      // Students tablosundaki AdvisorID, foreign key olarak atanır


            // Dersler (Courses) tablosu için birincil anahtar
            modelBuilder.Entity<Courses>().HasKey(c => c.CourseID);

            // Course -> CourseSelections ilişkisi
            modelBuilder.Entity<Courses>()
                .HasMany(c => c.CourseSelections)
                .WithOne(cs => cs.Course)
                .HasForeignKey(cs => cs.CourseID);


            // Ders Seçimleri (StudentCourseSelections) tablosu
            modelBuilder.Entity<StudentCourseSelections>().HasKey(scs => scs.SelectionID);

            modelBuilder.Entity<StudentCourseSelections>()
           .HasOne(scs => scs.Student)
           .WithMany(s => s.CourseSelections)
           .HasForeignKey(scs => scs.StudentID);

            modelBuilder.Entity<StudentCourseSelections>()
                .HasOne(scs => scs.Course)
                .WithMany(c => c.CourseSelections)
                .HasForeignKey(scs => scs.CourseID);

            // Transkriptler (Transcripts) tablosu
            modelBuilder.Entity<Transcripts>()
          .HasKey(t => t.TranscriptID);  // Primary Key tanımlaması

            // Transcript'ın öğrenciyle olan ilişkisini belirt
            modelBuilder.Entity<Transcripts>()
                .HasOne(t => t.Student)   // Bir transkriptin bir öğrencisi var
                .WithMany(s => s.Transcripts) // Öğrencinin birden fazla transkripti olabilir
                .HasForeignKey(t => t.StudentID); // Foreign Key'i belirt

        // Transcript'ın dersle olan ilişkisini belirt
        modelBuilder.Entity<Transcripts>()
            .HasOne(t => t.Course)   // Bir transkriptin bir dersi var
            .WithMany(c => c.Transcripts) // Dersin birden fazla transkripti olabilir
            .HasForeignKey(t => t.CourseID); // Foreign Key'i belirt

            // Öğrenciler ve Kullanıcılar (Students - Users) arasında birebir ilişki
            modelBuilder.Entity<Students>()
                .HasOne(s => s.User)               // Bir Öğrenci'nin bir Kullanıcısı vardır
                .WithOne(u => u.Student)           // Bir Kullanıcı'nın bir Öğrencisi vardır
                .HasForeignKey<Students>(s => s.UserID) // Öğrenci tablosundaki UserID, foreign key olarak ayarlanır
                .OnDelete(DeleteBehavior.Cascade); // Kullanıcı silinirse ilişkili öğrenci de silinir

            // Öğrenciler tablosu için zorunlu alanlar
            modelBuilder.Entity<Students>()
                .Property(s => s.FirstName)
                .IsRequired();

            modelBuilder.Entity<Students>()
                .Property(s => s.LastName)
                .IsRequired();

            modelBuilder.Entity<Students>()
                .Property(s => s.Email)
                .IsRequired();

            // Dersler tablosu için zorunlu alanlar
            modelBuilder.Entity<Courses>()
                .Property(c => c.CourseName)
                .IsRequired();

            modelBuilder.Entity<Courses>()
                .Property(c => c.CourseCode)
                .IsRequired();

            // Classes tablosu için birincil anahtar
            modelBuilder.Entity<Classes>()
                .HasKey(c => c.ClassID); // ClassID'yi birincil anahtar olarak belirledim

            // Classes tablosu için zorunlu alanlar
            modelBuilder.Entity<Classes>()
                .Property(c => c.ClassName)
                .IsRequired();

            modelBuilder.Entity<Classes>()
                .Property(c => c.Department)
                .IsRequired();

            // ClassCourseMappings için ilişki ve zorunlu alanlar
            modelBuilder.Entity<ClassCourseMappings>()
                .HasKey(ccm => ccm.MappingID); // MappingID'yi birincil anahtar olarak belirledim

            // ClassCourseMappings ve Classes arasındaki ilişki
            modelBuilder.Entity<ClassCourseMappings>()
                .HasOne(ccm => ccm.Class)  // ClassCourseMappings'in Class ile ilişkisi var
                .WithMany()  // Bir Class birden fazla ClassCourseMappings'e sahip olabilir
                .HasForeignKey(ccm => ccm.ClassID)  // ClassID ile ilişkilendirilir
                .OnDelete(DeleteBehavior.Cascade);  // Class silindiğinde ilişkili Mapping de silinir

            // ClassCourseMappings ve Courses arasındaki ilişki
            modelBuilder.Entity<ClassCourseMappings>()
                .HasOne(ccm => ccm.Course)  // ClassCourseMappings'in Course ile ilişkisi var
                .WithMany()  // Bir Course birden fazla ClassCourseMappings'e sahip olabilir
                .HasForeignKey(ccm => ccm.CourseID)  // CourseID ile ilişkilendirilir
                .OnDelete(DeleteBehavior.Cascade);  // Course silindiğinde ilişkili Mapping de silinir

            base.OnModelCreating(modelBuilder);
        }
    }
}
