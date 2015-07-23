﻿namespace Crawler.Data.DbContext
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity;
    using System.Linq;
    using System.Data.Entity.Validation;
    using MySql.Data.Entity;

    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class CrawlerStorage : DbContext
    {
        // 컨텍스트가 응용 프로그램의 구성 파일(App.config 또는 Web.config)의 'CrawlerStorage' 연결 문자열을 
        // 사용하도록 구성되었습니다. 기본적으로 이 연결 문자열은 LocalDb 인스턴스의  
        // 'Crawler.Data.DbContext.CrawlerStorage' 데이터베이스를 대상으로 합니다. 
        // 
        // 다른 데이터베이스 및/또는 데이터베이스 공급자를 대상으로 할 경우 응용 프로그램 구성 파일에서 'CrawlerStorage' 
        // 연결 문자열을 수정하십시오.
        public CrawlerStorage()
            : base("name=CrawlerStorage")
        {
            var ensureDLLIsCopied = System.Data.Entity.SqlServer.SqlProviderServices.Instance;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            //modelBuilder.Entity<Snapshot>()
            //    .HasOptional(s => s.Board)
            //    .WithOptionalDependent()
            //    .WillCascadeOnDelete(false);
        }

        // 모델에 포함할 각 엔터티 형식에 대한 DbSet을 추가합니다. Code First 모델 구성 및 사용에 대한 
        // 자세한 내용은 http://go.microsoft.com/fwlink/?LinkId=390109를 참조하십시오.

        public virtual DbSet<Board> Boards { get; set; }
        public virtual DbSet<Content> Contents { get; set; }
        public virtual DbSet<Snapshot> Snapshots { get; set; }
        public virtual DbSet<ContentRevision> ContentRevisions { get; set; }
        public virtual DbSet<SnapshotToContentRevision> SnapshotToContentRevisions { get; set; }
        public virtual DbSet<Srcdata> Srcdatas { get; set; }
        public virtual DbSet<TimePeriod> TimePeriods { get; set; }
        public virtual DbSet<Website> Websites { get; set; }
        public virtual DbSet<ErrorLog> ErrorLogs { get; set; }
    }
    public class Website
    {
        [Key]
        [Column("_websiteid")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required]
        [Column("label")]
        [StringLength(50)]
        public string Label { get; set; }

        [Required]
        [Column("website_url")]
        public string Website_URL { get; set; }

        [Required]
        [Column("mobile_url")]
        public string Mobile_URL { get; set; }

        [StringLength(250)]
        [Column("website_logo")]
        public string website_logo { get; set; }

        public virtual ICollection<Board> Boards { get; set; }
    }

    public class Board
    {
        [Key]
        [Column("_boardid")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Column("for_websiteid")]
        public int For_WebsiteId { get; set; }

        [Required]
        [StringLength(50)]
        [Column("label")]
        public string Label { get; set; }

        [ForeignKey("For_WebsiteId")]
        public virtual Website Website { get; set; }

        public virtual ICollection<Snapshot> Snapshots { get; set; }
    }
    public class TimePeriod
    {

        [Key]
        [Column("_timeperiodid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("shortguid")]
        public Guid ShortGuid { get; set; }

        [Column("label")]
        public string Label { get; set; }

        [Required]
        [Column("scheduled")]
        public DateTime Scheduled { get; set; }

        [Column("crawled")]
        public DateTime Crawled { get; set; }
        public virtual ICollection<Snapshot> Snapshots { get; set; }
    }
    public class Snapshot
    {
        [Key]
        [Column("_snapshotid")]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("for_boardid")]
        public int For_BoardId { get; set; }

        [Column("for_timeperiodid")]
        public int For_Timeperiod { get; set; }

        [Column("taken")]
        public DateTime Taken { get; set; }

        [ForeignKey("For_BoardId")]
        public virtual Board Board { get; set; }

        [ForeignKey("For_Timeperiod")]
        public virtual TimePeriod TimePeriod { get; set; }

        public virtual ICollection<SnapshotToContentRevision> SnapshotToContentRevisions { get; set; }
    }
    public class SnapshotToContentRevision
    {
        [Key]
        [Column("_snapshottocontentrevisionid")]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("for_snapshotid")]
        public int For_SnapshotId { get; set; }

        [Column("seqno")]
        public int Seqno { get; set; }

        [Column("has_contentrevisionid")]
        public int Has_ContentRevisionId { get; set; }

        [ForeignKey("For_SnapshotId")]
        public virtual Snapshot Snapshot { get; set; }

        [ForeignKey("Has_ContentRevisionId")]
        public virtual ContentRevision ContentRevision { get; set; }
    }
    public class ContentRevision
    {
        [Key]
        [Column("_contentrevisionid")]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("crawled")]
        public DateTime Crawled { get; set; }

        [StringLength(24)]
        [Column("checksum")]
        public string CheckSum { get; set; }

        [Column("details")]
        public string Details { get; set; }

        [Column("details_html")]
        public string Details_Html { get; set; }

        [Column("recommand_count")]
        public int RecommandCount { get; set; }

        [Column("view_count")]
        public int ViewCount { get; set; }

        [Column("is_depricate")]
        public bool IsDepricated { get; set; }

        [Column("for_contentid")]
        public int For_ContentId { get; set; }

        [ForeignKey("For_ContentId")]
        public virtual Content Content { get; set; }

        public virtual ICollection<SnapshotToContentRevision> SnapshotToContentRevisions { get; set; }
    }
    public class Content
    {
        [Key]
        [Column("_contentid")]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("contentguid")]
        public Guid ContentGuId { get; set; }

        [Column("url_params")]
        public string Url_Params { get; set; }

        [Required]
        [Column("contents_url")]
        public string Contents_URL { get; set; }

        [Column("checksum")]
        public string CheckSum { get; set; }

        [Required]
        [Column("article")]
        public string Article { get; set; }

        public virtual ICollection<ContentRevision> ContentRevisions { get; set; }
        public virtual ICollection<Srcdata> Srcdatas { get; set; }
    }
    public class Srcdata
    {
        [Key]
        [Column("_srcdataid")]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("srcguid")]
        public Guid SrcGuId { get; set; }

        [Column("for_contentid")]
        public int For_ContentId { get; set; }

        [StringLength(250)]
        [Column("original_sourceurl", TypeName = "VARCHAR")]
        public string Original_SourceUrl { get; set; }

        [StringLength(250)]
        [Column("private_sourceurl", TypeName = "VARCHAR")]
        public string private_SourceUrl { get; set; }

        [Required]
        [Column("originalpayload_size")]
        public long OriginalPayload_Size { get; set; }

        [Column("processedpayload_size")]
        public long? ProcessedPayload_Size { get; set; }

        [StringLength(24)]
        [Column("checksum")]
        public string CheckSum { get; set; }

        [StringLength(250)]
        [Column("filename", TypeName = "VARCHAR")]
        public string FileName { get; set; }

        [Column("originalpayload")]
        public byte[] OriginalPayload { get; set; }

        [Column("processedpayload")]
        public byte[] ProcessedPayload { get; set; }

        [Column("is_depricate")]
        public bool IsDepricated { get; set; }

        [ForeignKey("For_ContentId")]
        public virtual Content Content { get; set; }
    }

    public class ErrorLog
    {
        [Key]
        [Column("_errorlogid")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Column("error_address")]
        public string Error_Address { get; set; }

        [Column("error_url")]
        public string Error_URL { get; set; }

        [Column("error_details")]
        public string Error_Details { get; set; }

        [Column("hresult")]
        public int Hresult { get; set; }
    }
}