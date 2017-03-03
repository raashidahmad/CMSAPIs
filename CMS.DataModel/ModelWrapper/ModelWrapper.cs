using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.DataModel.ModelWrapper
    {

    public class ActionResponse
        {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int? ReturnedId { get; set; }
        }

    public class District
        {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SMSPrefix { get; set; }
        }

    public class Category
        {
        public int Id { get; set; }
        public string Name { get; set; }
        }

    public class SDC
        {
        public int Id { get; set; }
        public string Title { get; set; }
        public int DistrictId { get; set; }
        }

    public class Role
        {
        public string Id { get; set; }
        public string Name { get; set; }
        }

    public class SDCView
        {
        public int Id { get; set; }
        public string Title { get; set; }
        public string District { get; set; }
        public int DistrictId { get; set; }
        }

    public class Complainant
        {
        public enum CommunicationMedium : int
            {
            SMS = 1,
            Phone = 2
            }

        public int Id { get; set; }
        public string FullName { get; set; }
        public string NIC { get; set; }
        public string Address { get; set; }
        public string Mobile { get; set; }
        public CommunicationMedium ContactMedium { get; set; }
        }

    public class ComplaintView
        {
        public enum ComplaintStatus : int
            {
            InQueue = 1,
            UnderProcess = 2,
            Completed = 3,
            Closed = 4
            }

        public int Id { get; set; }
        public string Complainant { get; set; }
        public int CategoryId { get; set; }
        public string Category { get; set; }
        public string District { get; set; }
        public string SDC { get; set; }
        public string Description { get; set; }
        public DateTime Dated { get; set; }
        public ComplaintStatus Status { get; set; }
        public List<string> Documents { get; set; }
        }

    public class ComplaintFullView
        {
        public enum ComplaintStatus : int
            {
            InQueue = 1,
            UnderProcess = 2,
            Completed = 3,
            Closed = 4
            }

        public enum CommunicationMedium : int
            {
            SMS = 1,
            Phone = 2
            }

        public int Id { get; set; }
        public int ComplainantId { get; set; }
        public string Complainant { get; set; }
        public string ComplainantNIC { get; set; }
        public string ComplainantMobile { get; set; }
        public string ComplainantAddress { get; set; }
        public CommunicationMedium ContactMedium { get; set; }
        public string Category { get; set; }
        public string District { get; set; }
        public string SDC { get; set; }
        public string Description { get; set; }
        public DateTime Dated { get; set; }
        public ComplaintStatus Status { get; set; }
        public List<string> Documents { get; set; }
        }

    public class NewComplaint
        {
        public enum ComplaintStatus : int
            {
            InQueue = 1,
            UnderProcess = 2,
            Completed = 3,
            Closed = 4
            }
        public int ComplainantId { get; set; }
        public int CategoryId { get; set; }
        public int DistrictId { get; set; }
        public int SDCId { get; set; }
        public string Description { get; set; }
        public List<string> Documents { get; set; }
        }

    public class ComplainantList
        {
        public int Id { get; set; }
        public string NIC { get; set; }
        public string FullName { get; set; }
        }

    public class Documents
        {
        public int Id { get; set; }
        public int ComplaintId { get; set; }
        public string Path { get; set; }
        }

    public class SMSLogs
        {
        public int Id { get; set; }
        public int ComplaintId { get; set; }
        public int ComplainantId { get; set; }
        public string Text { get; set; }
        public DateTime Dated { get; set; }
        }

    public class UserView
        {
        public string Username { get; set; }
        public string FullName { get; set; }
        public int DistrictId { get; set; }
        public string District { get; set; }
        public int SDCId { get; set; }
        public string SDC { get; set; }
        public List<string> Roles { get; set; }
        }
    
    public class FilterIds
        {
        public int Id { get; set; }
        }

    public class DocumentEntry
        {
        public int EntryId { get; set; }
        public string Document { get; set; }
        }

    }
