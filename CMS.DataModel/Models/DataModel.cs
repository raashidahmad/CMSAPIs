using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity.EntityFramework;

namespace CMS.DataModel.Models
    {

    public class EFDistrict
        {
        [Required]
        public int Id { get; set; }
        public string Name { get; set; }
        public string SMSPrefix { get; set; }
        }

    public class EFCategory
        {
        [Required]
        public int Id { get; set; }
        public string Name { get; set; }
        }

    public class EFSDC
        {
        public int Id { get; set; }
        public string Title { get; set; }
        public EFDistrict District { get; set; }
        }

    public class EFComplainant
        {
        public enum CommunicationMedium : int
            {
            SMS = 1,
            Phone = 2
            }

        [Required]
        public int Id { get; set; }
        public string FullName { get; set; }
        public string NIC { get; set; }
        public string Address { get; set; }
        public string Mobile { get; set; }
        public CommunicationMedium ContactMedium { get; set; }
        }

    public class EFComplaint
        {
        public enum ComplaintStatus : int
            {
            InQueue = 1,
            UnderProcess = 2,
            Completed = 3,
            Closed = 4
            }

        public EFComplaint()
            {
            this.SMSLogs = new HashSet<EFSMSLogs>();
            this.Documents = new HashSet<EFDocuments>();
            this.PhoneLogs = new HashSet<EFPhoneCallLogs>();
            }

        [Required]
        public int Id { get; set; }
        [Required]
        public EFComplainant Complainant { get; set; }
        public EFCategory Category { get; set; }
        public EFSDC SDC { get; set; }
        public string Description { get; set; }
        public DateTime Dated { get; set; }
        public ComplaintStatus Status { get; set; }
        public ICollection<EFSMSLogs> SMSLogs { get; set; }
        public ICollection<EFDocuments> Documents { get; set; }
        public ICollection<EFPhoneCallLogs> PhoneLogs { get; set; }
        }

    public class EFDocuments
        {
        [Required]
        public int Id { get; set; }
        [Required]
        public EFComplaint Complaint { get; set; }
        [Required]
        public string Path { get; set; }
        }

    public class EFSMSLogs
        {
        public int Id { get; set; }
        public EFComplaint Complain { get; set; }
        public EFComplainant Complainant { get; set; }
        public string Text { get; set; }
        public DateTime Dated { get; set; }
        }

    public class EFPhoneCallLogs
        {
        public int Id { get; set; }
        public EFComplaint Complain { get; set; }
        public EFComplainant Complainant { get; set; }
        public DateTime Dated { get; set; }
        }
    
    }
