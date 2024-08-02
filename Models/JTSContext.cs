using System;
using System.Collections.Generic;
using System.Globalization;
using BusinessIntelligence_API.DAL;
using BusinessIntelligence_API.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Hosting;

namespace BusinessIntelligence_API.Models
{
    public partial class JTSContext : DbContext
    {	

		public JTSContext()
        {
			
		}

        public JTSContext(DbContextOptions<JTSContext> options)
            : base(options)
        {
			
		}

		public virtual DbSet<BiApiCallLog> BiApiCallLogs { get; set; } = null!;
		public virtual DbSet<BiChildmenu> BiChildmenus { get; set; } = null!;
		public virtual DbSet<BiMenuInventory> BiMenuInventories { get; set; } = null!;
		public virtual DbSet<BiParentMenu> BiParentMenus { get; set; } = null!;
		public virtual DbSet<BiPerformanceMetric> BiPerformanceMetrics { get; set; } = null!;
		public virtual DbSet<BiPublisher> BiPublishers { get; set; } = null!;
		public virtual DbSet<BiPublisherConfig> BiPublisherConfigs { get; set; } = null!;
		public virtual DbSet<BiRoleMaster> BiRoleMasters { get; set; } = null!;
		public virtual DbSet<BiUserMaster> BiUserMasters { get; set; } = null!;
		public virtual DbSet<BiCustomerDatum> BiCustomerData { get; set; } = null!;
		public virtual DbSet<BiDirectCost> BiDirectCosts { get; set; } = null!;
		public virtual DbSet<BiForex> BiForices { get; set; } = null!;
		public virtual DbSet<BiIndirectLabourCost> BiIndirectLabourCosts { get; set; } = null!;
		public virtual DbSet<BiOtherCost> BiOtherCosts { get; set; } = null!;
		public virtual DbSet<BiDashboardChart> BiDashboardChart { get; set; } = null!;
		public virtual DbSet<BiDashboardMetricChart> BiDashboardMetricCharts { get; set; } = null!;

		public virtual DbSet<BiDashboardMonthyearChart> BiDashboardMonthyearCharts { get; set; } = null!;
		//annapandiyan
		public virtual DbSet<BiQmsDatum> BiQmsData { get; set; } = null!;
		public DbSet<BidQmsDataDashboardReport> SSP_BidQmsDataDashboardReport { get; set; } = null!;
		public DbSet<BIQMSFeedbackDashboardReport> SSP_BIQMSFeedbackDashboardReport { get; set; } = null!;
		public virtual DbSet<BiProcessMst> BiProcessMsts { get; set; } = null!;
		public virtual DbSet<BiFinCusDefSheet> BiFinCusDefSheets { get; set; } = null!;
		//-------end -----------
		public DbSet<SSP_BIReport_DetailedResult> SSP_BIReportEntities { get; set; } = null!;
		public DbSet<SSP_BIReport_Shortfall> SSP_BIReportShortfallEntities { get; set; } = null!;
		//murugan
		public DbSet<BiUnderperforming> SSP_BIUnderPerformingEntities { get; set; } = null!;

		//finace chart 
		public DbSet<BiDirectCost> SSP_FinDash_DirectCostReport { get; set; } = null!;
		public DbSet<BiOtherCost> SSP_FinDash_OtherCostReport { get; set; } = null!;
		public DbSet<BiCustomerDatum> SSP_FinDash_CustomerDataReport { get; set; } = null!;
		public virtual DbSet<BiQmsFeedback> BiQmsFeedbacks { get; set; } = null!;

		public virtual DbSet<BiDepartmentMst> BiDepartmentMsts { get; set; } = null!;
		//GetFinCustomerDataReportProcedure
		//annapandiyan update
		//	public async Task<List<BiCustomerDatum>> GetCustomerDataByFilterProcedure(string InvoiceNo, DateTime InvoiceDate,
		//string CustomerName, string CustomerAcronym, string CCYType, string MajorHeadServiceLine)
		//	{
		//		// Assuming the stored procedure takes one INT parameter and returns rows from Employees
		//		return await BiCustomerData.FromSqlRaw("EXEC sp_bi_customerDataFilter {0},{1},{2},{3},{4},{5}", InvoiceNo, InvoiceDate, CustomerName, CustomerAcronym, CCYType, MajorHeadServiceLine).ToListAsync();
		//	}

		public async Task<IEnumerable<BidQmsDataDashboardReport>> GetQMSDataByFilterProcedure(BiQmsFilter param)
		{
			// Assuming the stored procedure takes one INT parameter and returns rows from Employees
			return await SSP_BidQmsDataDashboardReport.FromSqlRaw("EXEC SSP_QMS_Data_DashboardReport {0},{1},{2}", param.PublisherName, param.From, param.To).ToListAsync();
		}

		public async Task<IEnumerable<BIQMSFeedbackDashboardReport>> GetQMSFeedByFilterProcedure(BiQmsFilter param)
		{

			// Assuming the stored procedure takes one INT parameter and returns rows from Employees
			return await SSP_BIQMSFeedbackDashboardReport.FromSqlRaw("EXEC SSP_QMS_Feedback_DashboardReport {0}, {1} ,{2}", param.PublisherName, param.From, param.To).ToListAsync();

		}

		//store procedure
		public async Task<List<BiCustomerDatum>> GetCustomerDataByFilterProcedure(string InvoiceNo, DateTime InvoiceDate,
			   string CustomerName, string CustomerAcronym, string CCYType, string MajorHeadServiceLine)
		{
			// Assuming the stored procedure takes one INT parameter and returns rows from Employees
			return await BiCustomerData.FromSqlRaw("EXEC sp_bi_customerDataFilter {0},{1},{2},{3},{4},{5}", InvoiceNo, InvoiceDate, CustomerName, CustomerAcronym, CCYType, MajorHeadServiceLine).ToListAsync();
		}
		public async Task<List<BiDashboardMonthyearChart>> GetMonthYearChart(string sMonthyear, string eMonthyear)
		{
			// Assuming the stored procedure takes one INT parameter and returns rows from Employees
			return await BiDashboardMonthyearCharts.FromSqlRaw("EXEC SSP_BIReport {0},{1}", sMonthyear, eMonthyear).ToListAsync();
		}

		public async Task<List<BiDashboardChart>> GetLeadingReliableContributors(string Para1, string Para2, string Top,
		  string startmonthyear, string endmonthyear, string Publisher)
		{
			return await BiDashboardChart.FromSqlRaw("EXEC SSP_BIReport_Common {0},{1},{2},{3},{4},{5}", Para1, Para2, Top, startmonthyear, endmonthyear, Publisher).ToListAsync();
		}

		public async Task<List<BiDashboardChart>> GetUnderperformingPublishers(string Para1, string Para2, string bottom,
		  string startmonthyear, string endmonthyear, string Publisher)
		{
			return await BiDashboardChart.FromSqlRaw("EXEC SSP_BIReport_Common {0},{1},{2},{3},{4},{5}", Para1, Para2, bottom, startmonthyear, endmonthyear, Publisher).ToListAsync();
		}

		public async Task<List<BiDashboardMetricChart>> GetConsistentlyPerformingServices(string Para1, string Para2,
		   string startmonthyear, string endmonthyear, string top, string metric)
		{
			return await BiDashboardMetricCharts.FromSqlRaw("EXEC SSP_BIReport_Common {0},{1},{2},{3},{4},{5}", Para1, Para2, startmonthyear, endmonthyear, top, metric).ToListAsync();
		}
		public async Task<List<BiDashboardMetricChart>> GetUnderperformingServices(string Para1, string Para2, string Para3,
		  string startmonthyear, string endmonthyear, string Para4)
		{
			return await BiDashboardMetricCharts.FromSqlRaw("EXEC SSP_BIReport_Common {0},{1},{2},{3},{4},{5}", Para1, Para2, Para3, startmonthyear, endmonthyear, Para4).ToListAsync();
		}


		public async Task<List<BiDirectCost>> GetDirectCostByFilterProcedure(string TypeFilter, string DepartmentFilter,
	 string ServiceLineFilter, string CustomerFilter, string FCFilter, string BranchFilter)
		{
			// Assuming the stored procedure takes one INT parameter and returns rows from Employees
			return await BiDirectCosts.FromSqlRaw("EXEC sp_bi_directCostFilter {0},{1},{2},{3},{4},{5}", TypeFilter, DepartmentFilter, ServiceLineFilter, CustomerFilter, FCFilter, BranchFilter).ToListAsync();
		}

		//	public async Task<List<BiIndirectLabourCost>> GetInDirectCostByFilterProcedure(string DepartmentFilter,
		//string FCFilter, string BranchFilter)
		//	{
		//		// Assuming the stored procedure takes one INT parameter and returns rows from Employees
		//		return await BiIndirectLabourCosts.FromSqlRaw("EXEC sp_bi_IndirectCostFilter {0},{1},{2}", DepartmentFilter, FCFilter, BranchFilter).ToListAsync();
		//	}

		public async Task<List<BiIndirectLabourCost>> GetInDirectCostByFilterProcedure
(InDirectCostFilterParam param)
		{
			// Assuming the stored procedure takes one INT parameter and returns rows from Employees
			return await BiIndirectLabourCosts.FromSqlRaw("EXEC sp_bi_IndirectCostFilter {0},{1},{2}", param.DepartmentFilter, param.BranchFilter, param.FCFilter).ToListAsync();
		}

		//Finance chart Sp
		public async Task<List<BiDirectCost>> GetFinDashDirectCostReportProcedure
(DirectCostParam param)
		{			
			return await SSP_FinDash_DirectCostReport.FromSqlRaw("EXEC SSP_FinDash_DirectCostReport {0},{1},{2},{3},{4}", param.serviceLine, param.customerName, param.valueType, param.startMonth, param.endMonth).ToListAsync();
		}

		public async Task<List<BiOtherCost>> GetFinOtherCostReportProcedure
(DirectCostParam param)
		{
			return await SSP_FinDash_OtherCostReport.FromSqlRaw("EXEC SSP_FinDash_OtherCostReport {0},{1},{2},{3}", param.serviceLine, param.customerName, param.startMonth, param.endMonth).ToListAsync();
		}

		public async Task<List<BiCustomerDatum>> GetFinCustomerDataReportProcedure
(DirectCostParam param)
		{
			return await SSP_FinDash_CustomerDataReport.FromSqlRaw("EXEC SSP_FinDash_CustomerDataReport {0},{1},{2},{3},{4}", param.serviceLine, param.customerName, param.valueType, param.startMonth, param.endMonth).ToListAsync();
		}


		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
				string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
				if (environment == "UAT")
				{
					optionsBuilder.UseSqlServer("Server=dev-test-editgenie.ctrglcosg13x.ap-south-1.rds.amazonaws.com;User ID=kpidevapp;Password=Ytrew@18Dec23;Database=KPIAnalyticsDev;Trusted_Connection=False;");
				}				
				else
				{
					optionsBuilder.UseSqlServer("Server=sesame-dblive.ctrglcosg13x.ap-south-1.rds.amazonaws.com;User ID=kpiliveapp;Password=Alyp%2okiFbz4;Database=KPIAnalyticsLive;Trusted_Connection=False;");
				}
				
			}
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
			//modelBuilder.Entity<SSP_BIReport_DetailedResult>().HasNoKey();
			modelBuilder.Entity<SSP_BIReport_DetailedResult>().HasNoKey();
			modelBuilder.Entity<BidQmsDataDashboardReport>().HasNoKey();
			modelBuilder.Entity<BIQMSFeedbackDashboardReport>().HasNoKey();

			modelBuilder.Entity<BiApiCallLog>(entity =>
			{
				entity.ToTable("Bi_ApiCallLogs");

				entity.Property(e => e.Endpoint).HasMaxLength(255);

				entity.Property(e => e.Environment).HasMaxLength(50);

				entity.Property(e => e.Method).HasMaxLength(50);

				entity.Property(e => e.ServerName).HasMaxLength(255);

				entity.Property(e => e.Timestamp).HasColumnType("datetime");

				entity.Property(e => e.Username).HasMaxLength(255);
			});

			modelBuilder.Entity<BiChildmenu>(entity =>
			{
				entity.HasKey(e => e.MenuId);

				entity.ToTable("Bi_Childmenu");

				entity.Property(e => e.MenuId)
					.ValueGeneratedNever()
					.HasColumnName("menuId");

				entity.Property(e => e.CreatedDate)
					.HasColumnType("datetime")
					.HasColumnName("createdDate");

				entity.Property(e => e.Iactive).HasColumnName("iactive");

				entity.Property(e => e.Icon)
					.HasMaxLength(50)
					.HasColumnName("icon");

				entity.Property(e => e.Id)
					.ValueGeneratedOnAdd()
					.HasColumnName("id");

				entity.Property(e => e.Label)
					.HasMaxLength(50)
					.HasColumnName("label");

				entity.Property(e => e.Link)
					.HasMaxLength(50)
					.HasColumnName("link");

				entity.Property(e => e.MenuName)
					.HasMaxLength(50)
					.HasColumnName("menuName");

				entity.Property(e => e.ParentId).HasColumnName("parentId");

				entity.Property(e => e.UpdatedDate)
					.HasColumnType("datetime")
					.HasColumnName("updatedDate");
			});

			modelBuilder.Entity<BiMenuInventory>(entity =>
			{
				entity.ToTable("Bi_MenuInventory");

				entity.Property(e => e.Action)
					.HasMaxLength(50)
					.HasColumnName("action");

				entity.Property(e => e.ChildmenuId)
					.HasMaxLength(50)
					.HasColumnName("childmenu_id");

				entity.Property(e => e.InsertedTime)
					.HasColumnType("datetime")
					.HasColumnName("inserted_time");

				entity.Property(e => e.ParentmenuId)
					.HasMaxLength(250)
					.HasColumnName("parentmenu_id");

				entity.Property(e => e.canview).HasColumnName("canview");

				entity.Property(e => e.caninsert).HasColumnName("caninsert");

				entity.Property(e => e.canupdate).HasColumnName("canupdate");

				entity.Property(e => e.candelete).HasColumnName("candelete");

				entity.Property(e => e.UpdatedTime)
					.HasColumnType("datetime")
					.HasColumnName("updated_time");

				entity.Property(e => e.UserroleId)
					.HasMaxLength(50)
					.HasColumnName("userrole_id");
			});

			modelBuilder.Entity<BiParentMenu>(entity =>
			{
				entity.HasKey(e => e.MenuId)
					.HasName("PK_Bi_ParentMenu_1");

				entity.ToTable("Bi_ParentMenu");

				entity.Property(e => e.MenuId)
					.ValueGeneratedNever()
					.HasColumnName("menuId");

				entity.Property(e => e.CreatedDate)
					.HasColumnType("datetime")
					.HasColumnName("createdDate");

				entity.Property(e => e.Iactive).HasColumnName("iactive");

				entity.Property(e => e.Icon)
					.HasMaxLength(50)
					.HasColumnName("icon");

				entity.Property(e => e.Id)
					.ValueGeneratedOnAdd()
					.HasColumnName("id");

				entity.Property(e => e.Label)
					.HasMaxLength(50)
					.HasColumnName("label");

				entity.Property(e => e.Link)
					.HasMaxLength(50)
					.HasColumnName("link");

				entity.Property(e => e.MenuName)
					.HasMaxLength(50)
					.HasColumnName("menuName");

				entity.Property(e => e.UpdatedDate)
					.HasColumnType("datetime")
					.HasColumnName("updatedDate");
			});

			modelBuilder.Entity<BiPerformanceMetric>(entity =>
			{
				entity.ToTable("Bi_PerformanceMetrics");

				entity.Property(e => e.Id).HasColumnName("ID");

				entity.Property(e => e.AccountManagement).HasColumnType("decimal(5, 2)");

				entity.Property(e => e.Acronym).HasMaxLength(50);

				entity.Property(e => e.AuthorSatisfaction).HasColumnType("decimal(5, 2)");

				entity.Property(e => e.Communication).HasColumnType("decimal(5, 2)");

				entity.Property(e => e.CustomerSatisfaction).HasColumnType("decimal(5, 2)");

				entity.Property(e => e.Feedback).HasColumnType("decimal(5, 2)");

				entity.Property(e => e.InsertedDate).HasColumnType("datetime");

				entity.Property(e => e.MonthYear).HasMaxLength(10);

				entity.Property(e => e.OverallPerformance).HasColumnType("decimal(5, 2)");

				entity.Property(e => e.PublicationSpeed).HasColumnType("decimal(5, 2)");

				entity.Property(e => e.Publisher).HasMaxLength(150);

				entity.Property(e => e.PublisherId)
					.HasMaxLength(10)
					.HasColumnName("PublisherID");

				entity.Property(e => e.Quality).HasColumnType("decimal(5, 2)");

				entity.Property(e => e.Rft)
					.HasColumnType("decimal(5, 2)")
					.HasColumnName("RFT");

				entity.Property(e => e.Schedule).HasColumnType("decimal(5, 2)");

				entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
			});

			modelBuilder.Entity<BiPublisher>(entity =>
			{
				entity.ToTable("Bi_Publisher");

				entity.Property(e => e.Id).HasColumnName("ID");

				entity.Property(e => e.Acronym).HasMaxLength(50);

				entity.Property(e => e.InsertedDate).HasColumnType("datetime");

				entity.Property(e => e.PublisherName).HasMaxLength(100);

				entity.Property(e => e.UpdatedDate)
					.HasColumnType("datetime")
					.HasColumnName("updatedDate");
			});

			modelBuilder.Entity<BiPublisherConfig>(entity =>
			{
				entity.ToTable("Bi_PublisherConfig");

				entity.Property(e => e.Id).HasColumnName("ID");

				entity.Property(e => e.AccountManagementAction).HasMaxLength(50);

				entity.Property(e => e.AuthorsatisficationAction).HasMaxLength(50);

				entity.Property(e => e.CommunicationAction).HasMaxLength(50);

				entity.Property(e => e.CustomerSatisfactionAction).HasMaxLength(50);

				entity.Property(e => e.FeedbackAction).HasMaxLength(50);

				entity.Property(e => e.InsertedDate).HasColumnType("datetime");

				entity.Property(e => e.OverallPerfomanceAction).HasMaxLength(50);

				entity.Property(e => e.PublicationSpeedAction).HasMaxLength(50);

				entity.Property(e => e.PublisherName).HasMaxLength(50);

				entity.Property(e => e.QualityAction).HasMaxLength(50);

				entity.Property(e => e.RftAction)
					.HasMaxLength(50)
					.HasColumnName("RFTAction");

				entity.Property(e => e.RftMetrics).HasColumnName("RFTMetrics");

				entity.Property(e => e.RftRequired).HasColumnName("RFTRequired");

				entity.Property(e => e.ScheduleAction).HasMaxLength(50);

				entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
			});

			modelBuilder.Entity<BiRoleMaster>(entity =>
			{
				entity.HasKey(e => e.RoleId)
					.HasName("PK__Bi_RoleM__8AFACE3A272EED96");

				entity.ToTable("Bi_RoleMaster");

				entity.Property(e => e.RoleId).HasColumnName("RoleID");

				entity.Property(e => e.ControlCenter)
					.HasMaxLength(255)
					.IsUnicode(false);

				entity.Property(e => e.CreatedTime).HasColumnType("datetime");

				entity.Property(e => e.Description)
					.HasMaxLength(255)
					.IsUnicode(false);

				entity.Property(e => e.RoleName)
					.HasMaxLength(50)
					.IsUnicode(false);

				entity.Property(e => e.UpdatedTime).HasColumnType("datetime");
			});

			modelBuilder.Entity<BiUserMaster>(entity =>
			{
				entity.ToTable("Bi_UserMaster");

				entity.Property(e => e.Id).HasColumnName("id");

				entity.Property(e => e.Activestatus).HasColumnName("activestatus");

				entity.Property(e => e.Createdtime)
					.HasColumnType("datetime")
					.HasColumnName("createdtime");

				entity.Property(e => e.Displayname)
					.HasMaxLength(50)
					.IsUnicode(false)
					.HasColumnName("displayname");

				entity.Property(e => e.Emailid)
					.HasMaxLength(100)
					.IsUnicode(false)
					.HasColumnName("emailid");

				entity.Property(e => e.Firstname)
					.HasMaxLength(50)
					.IsUnicode(false)
					.HasColumnName("firstname");

				entity.Property(e => e.Lastname)
					.HasMaxLength(50)
					.IsUnicode(false)
					.HasColumnName("lastname");

				entity.Property(e => e.Password)
					.HasMaxLength(100)
					.IsUnicode(false)
					.HasColumnName("password");

				entity.Property(e => e.Signature)
					.HasMaxLength(50)
					.IsUnicode(false)
					.HasColumnName("signature");

				entity.Property(e => e.Updatedtime)
					.HasColumnType("datetime")
					.HasColumnName("updatedtime");

				entity.Property(e => e.Username)
					.HasMaxLength(50)
					.IsUnicode(false)
					.HasColumnName("username");

				entity.Property(e => e.Userrole)
					.HasMaxLength(50)
					.IsUnicode(false)
					.HasColumnName("userrole");
			});			

			modelBuilder.Entity<BiDirectCost>(entity =>
			{
				entity.ToTable("Bi_DirectCost");

				entity.Property(e => e.Branch).HasMaxLength(225);

				entity.Property(e => e.CostCtc)
					.HasColumnType("decimal(18, 0)")
					.HasColumnName("Cost_CTC");

				entity.Property(e => e.CreatedTime)
					.HasColumnType("datetime")
					.HasColumnName("createdTime");

				entity.Property(e => e.Customer).HasMaxLength(225);

				entity.Property(e => e.Department).HasMaxLength(225);

				entity.Property(e => e.Fc).HasColumnName("FC");

				entity.Property(e => e.FxRate)
					.HasColumnType("decimal(18, 0)")
					.HasColumnName("FX_Rate");

				entity.Property(e => e.NoOfManDate)
					.HasColumnType("decimal(18, 0)")
					.HasColumnName("No_Of_Man_Date");

				entity.Property(e => e.ServiceLine)
					.HasMaxLength(50)
					.HasColumnName("Service_Line");

				entity.Property(e => e.Type).HasMaxLength(50);

				entity.Property(e => e.UpdatedTime).HasColumnType("datetime");

				entity.Property(e => e.Year).HasColumnName("year");
			});

			modelBuilder.Entity<BiForex>(entity =>
			{
				entity.ToTable("Bi_Forex");

				entity.Property(e => e.CreatedTime)
					.HasColumnType("datetime")
					.HasColumnName("createdTime");

				entity.Property(e => e.Date)
					.HasColumnType("date")
					.HasColumnName("date");

				entity.Property(e => e.GbpInr)
					.HasColumnType("decimal(10, 3)")
					.HasColumnName("GBP_INR");

				entity.Property(e => e.PhpInr)
					.HasColumnType("decimal(10, 3)")
					.HasColumnName("PHP_INR");

				entity.Property(e => e.UpdatedTime).HasColumnType("datetime");

				entity.Property(e => e.UsdGbp)
					.HasColumnType("decimal(10, 3)")
					.HasColumnName("USD_GBP");

				entity.Property(e => e.UsdInr)
					.HasColumnType("decimal(10, 3)")
					.HasColumnName("USD_INR");
			});

			modelBuilder.Entity<BiIndirectLabourCost>(entity =>
			{
				entity.ToTable("Bi_Indirect_Labour_Cost");

				entity.Property(e => e.Branch).HasMaxLength(225);

				entity.Property(e => e.CostCtc)
					.HasColumnType("decimal(18, 0)")
					.HasColumnName("Cost_CTC");

				entity.Property(e => e.CreatedTime)
					.HasColumnType("datetime")
					.HasColumnName("createdTime");

				entity.Property(e => e.Department).HasMaxLength(225);

				entity.Property(e => e.Fc).HasColumnName("FC");

				entity.Property(e => e.FxRate)
					.HasColumnType("decimal(18, 0)")
					.HasColumnName("FX_Rate");

				entity.Property(e => e.NoOfManDate)
					.HasColumnType("decimal(18, 0)")
					.HasColumnName("No_Of_Man_Date");

				entity.Property(e => e.UpdatedTime).HasColumnType("datetime");

				entity.Property(e => e.Year).HasColumnName("year");
			});

			modelBuilder.Entity<BiOtherCost>(entity =>
			{
				entity.ToTable("Bi_OtherCost");

				entity.Property(e => e.BudgetedAmount).HasColumnType("decimal(18, 2)");

				entity.Property(e => e.Cgst)
					.HasColumnType("decimal(18, 2)")
					.HasColumnName("CGST");

				entity.Property(e => e.CreatedTime)
					.HasColumnType("datetime")
					.HasColumnName("createdTime");

				entity.Property(e => e.Fxrate)
					.HasColumnType("decimal(18, 2)")
					.HasColumnName("FXRate");

				entity.Property(e => e.HsnSac)
					.HasColumnType("numeric(8, 0)")
					.HasColumnName("HSN_SAC");

				entity.Property(e => e.Igst)
					.HasColumnType("decimal(18, 2)")
					.HasColumnName("IGST");

				entity.Property(e => e.InvoiceDate).HasColumnType("date");

				entity.Property(e => e.InvoiceNo).HasMaxLength(16);

				entity.Property(e => e.PoDate)
					.HasColumnType("date")
					.HasColumnName("PO_Date");

				entity.Property(e => e.PoNo)
					.HasMaxLength(16)
					.HasColumnName("PO_No");

				//entity.Property(e => e.Qty).HasColumnName("QTY");
				entity.Property(e => e.Qty)
				   .HasColumnType("decimal(10, 2)")
				   .HasColumnName("QTY");

				entity.Property(e => e.Rate).HasColumnType("decimal(10, 2)");

				entity.Property(e => e.Rcm).HasColumnName("RCM");

				entity.Property(e => e.ServiceLine).HasMaxLength(50);

				entity.Property(e => e.Sgst)
					.HasColumnType("decimal(18, 2)")
					.HasColumnName("SGST");

				entity.Property(e => e.TdsApplicable).HasColumnName("TDS_Applicable");

				entity.Property(e => e.TdsDeclaration).HasColumnName("TDS_Declaration");

				entity.Property(e => e.TdsRate)
					.HasColumnType("decimal(5, 2)")
					.HasColumnName("TDS_Rate");

				entity.Property(e => e.TdsSection)
					.HasMaxLength(255)
					.HasColumnName("TDS_Section");

				entity.Property(e => e.TdsValue)
					.HasColumnType("decimal(18, 2)")
					.HasColumnName("TDS_Value");

				entity.Property(e => e.TotalInvoiceValueInr)
					.HasColumnType("decimal(18, 2)")
					.HasColumnName("TotalInvoiceValue_INR");

				entity.Property(e => e.TypeOfExpense).HasMaxLength(50);

				entity.Property(e => e.UpdatedTime).HasColumnType("datetime");

				entity.Property(e => e.Value).HasColumnType("decimal(18, 2)");

				entity.Property(e => e.ValueInr)
					.HasColumnType("decimal(18, 2)")
					.HasColumnName("Value_INR");

				entity.Property(e => e.Variance).HasColumnType("decimal(18, 2)");

				entity.Property(e => e.Vat)
					.HasColumnType("decimal(18, 2)")
					.HasColumnName("VAT");

				entity.Property(e => e.Vch).HasColumnName("vch#");
			});

			
			modelBuilder.Entity<BiCustomerDatum>(entity =>
            {
                entity.ToTable("Bi_CustomerData");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AdBank).HasColumnName("AD_Bank");

                entity.Property(e => e.AgedDays)
                    .HasColumnType("decimal(10, 3)")
                    .HasColumnName("Aged_Days");

                entity.Property(e => e.Ccytype).HasColumnName("CCYType");

                entity.Property(e => e.CollectionDate).HasColumnType("date");

                entity.Property(e => e.CollectionFxrate)
                    .HasColumnType("decimal(10, 3)")
                    .HasColumnName("CollectionFXRate");

                entity.Property(e => e.CollectionValueFc)
                    .HasColumnType("decimal(10, 3)")
                    .HasColumnName("CollectionValue_FC");

                entity.Property(e => e.CollectionValueInr)
                    .HasColumnType("decimal(10, 3)")
                    .HasColumnName("CollectionValueINR");

                entity.Property(e => e.CreatedTime)
                    .HasColumnType("datetime")
                    .HasColumnName("createdTime");

                entity.Property(e => e.EbrcDate)
                    .HasColumnType("date")
                    .HasColumnName("EBRC_Date");

                entity.Property(e => e.EbrcNo).HasColumnName("EBRC_No");

                entity.Property(e => e.EdpmsClosureYnp).HasColumnName("EDPMS_Closure_YNP");

                entity.Property(e => e.EdpmsRefNo).HasColumnName("EDPMS_Ref_No");

                entity.Property(e => e.EdpmsUploadDate)
                    .HasColumnType("date")
                    .HasColumnName("EDPMS_Upload_Date");

                entity.Property(e => e.ForexGainLoss)
                    .HasColumnType("decimal(10, 3)")
                    .HasColumnName("Forex_Gain_Loss");

                entity.Property(e => e.GrossValueFc)
                    .HasColumnType("decimal(10, 3)")
                    .HasColumnName("GrossValue_FC");

                entity.Property(e => e.GrossValueInr)
                    .HasColumnType("decimal(10, 3)")
                    .HasColumnName("GrossValue_INR");

                entity.Property(e => e.InvoiceDate).HasColumnType("date");

                entity.Property(e => e.InvoiceFxrate)
                    .HasColumnType("decimal(10, 3)")
                    .HasColumnName("InvoiceFXRate");

                entity.Property(e => e.Irm).HasColumnName("IRM");

                entity.Property(e => e.MajorHeadServiceLine).HasColumnName("MajorHead_ServiceLine");

                entity.Property(e => e.NetValue).HasColumnType("decimal(10, 3)");

                entity.Property(e => e.NewBusiness).HasColumnName("New_Business");

                entity.Property(e => e.Quantity).HasColumnType("decimal(10, 3)");

                entity.Property(e => e.Rate).HasColumnType("decimal(10, 3)");

                entity.Property(e => e.SoftexNo).HasColumnName("Softex_No");

                entity.Property(e => e.StpiSubmissionDate)
                    .HasColumnType("date")
                    .HasColumnName("STPI_Submission_Date");

                entity.Property(e => e.Tspage)
                    .HasColumnType("decimal(10, 3)")
                    .HasColumnName("TSPage");

                entity.Property(e => e.UA).HasColumnName("U_A");

                entity.Property(e => e.Uom)
                    .HasMaxLength(50)
                    .HasColumnName("UOM");

                entity.Property(e => e.UpdatedTime).HasColumnType("datetime");

                entity.Property(e => e.Vat).HasColumnName("VAT");

                entity.Property(e => e.Vatvalue)
                    .HasColumnType("decimal(10, 3)")
                    .HasColumnName("VATValue");
            });


			modelBuilder.Entity<BiQmsFeedback>(entity =>
			{
				entity.ToTable("Bi_QMS_Feedback");

				entity.Property(e => e.Id).HasColumnName("id");

				entity.Property(e => e.Ce)
					.HasColumnType("decimal(4, 3)")
					.HasColumnName("CE");

				entity.Property(e => e.CreatedBy).HasColumnName("createdBy");

				entity.Property(e => e.CreatedOn)
					.HasColumnType("datetime")
					.HasColumnName("createdOn");

				entity.Property(e => e.IsDeleted)
					.HasColumnName("isDeleted")
					.HasDefaultValueSql("((0))");

				entity.Property(e => e.Mc)
					.HasColumnType("decimal(4, 3)")
					.HasColumnName("MC");

				entity.Property(e => e.MonthYear).HasMaxLength(10);

				entity.Property(e => e.NotNterror)
					.HasColumnType("decimal(4, 3)")
					.HasColumnName("NotNTerror");

				entity.Property(e => e.Pe)
					.HasColumnType("decimal(4, 3)")
					.HasColumnName("PE");

				entity.Property(e => e.Pm)
					.HasColumnType("decimal(4, 3)")
					.HasColumnName("PM");

				entity.Property(e => e.Positive).HasColumnType("decimal(4, 3)");

				entity.Property(e => e.PublisherName).HasMaxLength(20);

				entity.Property(e => e.Technical).HasColumnType("decimal(4, 3)");

				entity.Property(e => e.Total).HasColumnType("decimal(4, 3)");

				entity.Property(e => e.Typ)
					.HasColumnType("decimal(4, 3)")
					.HasColumnName("TYP");

				entity.Property(e => e.UpdatedBy).HasColumnName("updatedBy");

				entity.Property(e => e.UpdatedOn)
					.HasColumnType("datetime")
					.HasColumnName("updatedOn");

				entity.Property(e => e.Xml)
					.HasColumnType("decimal(4, 3)")
					.HasColumnName("XML");
			});
			modelBuilder.Entity<BiQmsDatum>(entity =>
			{
				entity.ToTable("Bi_QMS_Data");

				entity.Property(e => e.Id).HasColumnName("id");

				entity.Property(e => e.AuthorSurvey)
					.HasColumnType("numeric(18, 0)")
					.HasColumnName("Author_Survey");

				entity.Property(e => e.CeEpp)
					.HasColumnType("numeric(18, 0)")
					.HasColumnName("CE_EPP");

				entity.Property(e => e.CreatedBy).HasColumnName("createdBy");

				entity.Property(e => e.CreatedOn)
					.HasColumnType("datetime")
					.HasColumnName("createdOn");

				entity.Property(e => e.Epp)
					.HasColumnType("numeric(18, 0)")
					.HasColumnName("EPP");

				entity.Property(e => e.EppFp)
					.HasColumnType("numeric(18, 0)")
					.HasColumnName("EPP_FP");

				entity.Property(e => e.EppRev)
					.HasColumnType("numeric(18, 0)")
					.HasColumnName("EPP_Rev");

				entity.Property(e => e.Escalations).HasColumnType("numeric(18, 0)");

				entity.Property(e => e.Feedback).HasColumnType("numeric(18, 0)");

				entity.Property(e => e.IsDeleted)
					.HasColumnName("isDeleted")
					.HasDefaultValueSql("((0))");

				entity.Property(e => e.McEpp)
					.HasColumnType("numeric(18, 0)")
					.HasColumnName("MC_EPP");

				entity.Property(e => e.MonthYear).HasMaxLength(10);

				entity.Property(e => e.PeEpp)
					.HasColumnType("numeric(18, 0)")
					.HasColumnName("PE_EPP");

				entity.Property(e => e.Positive).HasColumnType("numeric(18, 0)");

				entity.Property(e => e.PublisherName).HasMaxLength(20);

				entity.Property(e => e.Rft)
					.HasColumnType("numeric(18, 0)")
					.HasColumnName("RFT");

				entity.Property(e => e.Ttp)
					.HasColumnType("numeric(18, 0)")
					.HasColumnName("TTP");

				entity.Property(e => e.TypEpp)
					.HasColumnType("numeric(18, 0)")
					.HasColumnName("TYP_EPP");

				entity.Property(e => e.UpdatedBy).HasColumnName("updatedBy");

				entity.Property(e => e.UpdatedOn)
					.HasColumnType("datetime")
					.HasColumnName("updatedOn");

				entity.Property(e => e.ZeroError).HasColumnType("numeric(18, 0)");
			});
			modelBuilder.Entity<BiQmsFeedback>(entity =>
			{
				entity.ToTable("Bi_QMS_Feedback");

				entity.Property(e => e.Id).HasColumnName("id");

				entity.Property(e => e.Ce)
					.HasColumnType("decimal(18, 0)")
					.HasColumnName("CE");

				entity.Property(e => e.CreatedBy).HasColumnName("createdBy");

				entity.Property(e => e.CreatedOn)
					.HasColumnType("datetime")
					.HasColumnName("createdOn");

				entity.Property(e => e.IsDeleted)
					.HasColumnName("isDeleted")
					.HasDefaultValueSql("((0))");

				entity.Property(e => e.Mc)
					.HasColumnType("decimal(18, 0)")
					.HasColumnName("MC");

				entity.Property(e => e.MonthYear).HasMaxLength(50);

				entity.Property(e => e.NotNterror)
					.HasColumnType("decimal(18, 0)")
					.HasColumnName("NotNTerror");

				entity.Property(e => e.Pe)
					.HasColumnType("decimal(18, 0)")
					.HasColumnName("PE");

				entity.Property(e => e.Pm)
					.HasColumnType("decimal(18, 0)")
					.HasColumnName("PM");

				entity.Property(e => e.Positive).HasColumnType("decimal(18, 0)");

				entity.Property(e => e.PublisherName).HasMaxLength(20);

				entity.Property(e => e.Technical).HasColumnType("decimal(18, 0)");

				entity.Property(e => e.Total).HasColumnType("decimal(18, 0)");

				entity.Property(e => e.Typ)
					.HasColumnType("decimal(18, 0)")
					.HasColumnName("TYP");

				entity.Property(e => e.UpdatedBy).HasColumnName("updatedBy");

				entity.Property(e => e.UpdatedOn)
					.HasColumnType("datetime")
					.HasColumnName("updatedOn");

				entity.Property(e => e.Xml)
					.HasColumnType("decimal(18, 0)")
					.HasColumnName("XML");
			});
			modelBuilder.Entity<BiDepartmentMst>(entity =>
			{
				entity.ToTable("Bi_Department_Mst");

				entity.Property(e => e.Id).HasColumnName("id");

				entity.Property(e => e.Acronym)
					.HasMaxLength(10)
					.HasColumnName("acronym");

				entity.Property(e => e.CreatedBy).HasColumnName("createdBy");

				entity.Property(e => e.CreatedOn)
					.HasColumnType("datetime")
					.HasColumnName("createdOn");

				entity.Property(e => e.IsDeleted)
					.HasColumnName("isDeleted")
					.HasDefaultValueSql("((0))");

				entity.Property(e => e.Title)
					.HasMaxLength(100)
					.HasColumnName("title");

				entity.Property(e => e.UpdatedBy).HasColumnName("updatedBy");

				entity.Property(e => e.UpdatedOn)
					.HasColumnType("datetime")
					.HasColumnName("updatedOn");
			});
			modelBuilder.Entity<BiQmsFeedback>(entity =>
			{
				entity.ToTable("Bi_QMS_Feedback");

				entity.Property(e => e.Id).HasColumnName("id");

				entity.Property(e => e.Ce)
					.HasColumnType("decimal(4, 3)")
					.HasColumnName("CE");

				entity.Property(e => e.CreatedBy).HasColumnName("createdBy");

				entity.Property(e => e.CreatedOn)
					.HasColumnType("datetime")
					.HasColumnName("createdOn");

				entity.Property(e => e.IsDeleted)
					.HasColumnName("isDeleted")
					.HasDefaultValueSql("((0))");

				entity.Property(e => e.Mc)
					.HasColumnType("decimal(4, 3)")
					.HasColumnName("MC");

				entity.Property(e => e.NotNterror)
					.HasColumnType("decimal(4, 3)")
					.HasColumnName("NotNTerror");

				entity.Property(e => e.Pe)
					.HasColumnType("decimal(4, 3)")
					.HasColumnName("PE");

				entity.Property(e => e.Pm)
					.HasColumnType("decimal(4, 3)")
					.HasColumnName("PM");

				entity.Property(e => e.Positive).HasColumnType("decimal(4, 3)");

				entity.Property(e => e.PublisherName).HasMaxLength(20);

				entity.Property(e => e.Technical).HasColumnType("decimal(4, 3)");

				entity.Property(e => e.Total).HasColumnType("decimal(4, 3)");

				entity.Property(e => e.Typ)
					.HasColumnType("decimal(4, 3)")
					.HasColumnName("TYP");

				entity.Property(e => e.UpdatedBy).HasColumnName("updatedBy");

				entity.Property(e => e.UpdatedOn)
					.HasColumnType("datetime")
					.HasColumnName("updatedOn");

				entity.Property(e => e.Xml)
					.HasColumnType("decimal(4, 3)")
					.HasColumnName("XML");
			});
			modelBuilder.Entity<BiProcessMst>(entity =>
			{
				entity.ToTable("Bi_Process_Mst");

				entity.Property(e => e.Id).HasColumnName("id");

				entity.Property(e => e.Acronym)
					.HasMaxLength(10)
					.HasColumnName("acronym");

				entity.Property(e => e.CreatedBy).HasColumnName("createdBy");

				entity.Property(e => e.CreatedOn)
					.HasColumnType("datetime")
					.HasColumnName("createdOn");

				entity.Property(e => e.IsDeleted)
					.HasColumnName("isDeleted")
					.HasDefaultValueSql("((0))");

				entity.Property(e => e.Title)
					.HasMaxLength(100)
					.HasColumnName("title");

				entity.Property(e => e.UpdatedBy).HasColumnName("updatedBy");

				entity.Property(e => e.UpdatedOn)
					.HasColumnType("datetime")
					.HasColumnName("updatedOn");
			});

			modelBuilder.Entity<BiFinCusDefSheet>(entity =>
			{
				entity.ToTable("Bi_FinCusDefSheet");

				entity.Property(e => e.Id).HasColumnName("id");

				entity.Property(e => e.Acronym).HasMaxLength(40);

				entity.Property(e => e.Ccy)
					.HasMaxLength(5)
					.HasColumnName("CCY");

				entity.Property(e => e.CreatedOn)
					.HasColumnType("date")
					.HasColumnName("createdOn");

				entity.Property(e => e.CustomerName).HasMaxLength(225);

				entity.Property(e => e.UpdatedOn)
					.HasColumnType("datetime")
					.HasColumnName("updatedOn");

				entity.Property(e => e.Vat).HasColumnName("VAT");

				entity.Property(e => e.Vatpercent)
					.HasColumnType("decimal(18, 0)")
					.HasColumnName("VATPercent")
					.HasDefaultValueSql("((20))");
			});

			OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
