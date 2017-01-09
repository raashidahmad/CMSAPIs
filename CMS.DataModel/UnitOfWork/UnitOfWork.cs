using CMS.DataModel.Models;
using CMS.DataModel.Repository;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.DataModel.UnitOfWork
    {
    public class UnitOfWork : IDisposable
        {
        private CMDbContext context = null;
        private GenericRepository<EFDistrict> districtRepository;
        private GenericRepository<EFCategory> categoryRepository;
        private GenericRepository<EFSDC> sdcRepository;
        private GenericRepository<EFComplainant> complainantRepository;
        private GenericRepository<EFComplaint> complaintRepository;
        private GenericRepository<EFDocuments> documentsRepository;

        public UnitOfWork()
            {
            context = new CMDbContext();
            }

        public GenericRepository<EFDistrict> DistrictRepository
            {
            get
                {
                if (this.districtRepository == null)
                    this.districtRepository = new GenericRepository<EFDistrict>(context);
                return districtRepository;
                }
            }

        public GenericRepository<EFCategory> CategoryRepository
            {
            get
                {
                if (this.categoryRepository == null)
                    this.categoryRepository = new GenericRepository<EFCategory>(context);
                return categoryRepository;
                }
            }

        public GenericRepository<EFSDC> SDCRepository
            {
            get
                {
                if (this.sdcRepository == null)
                    this.sdcRepository = new GenericRepository<EFSDC>(context);
                return sdcRepository;
                }
            }

        public GenericRepository<EFComplainant> ComplainantRepository
            {
            get
                {
                if (this.complainantRepository == null)
                    this.complainantRepository = new GenericRepository<EFComplainant>(context);
                return complainantRepository;
                }
            }

        public GenericRepository<EFComplaint> ComplaintRepository
            {
            get
                {
                if (this.complaintRepository == null)
                    this.complaintRepository = new GenericRepository<EFComplaint>(context);
                return complaintRepository;
                }
            }

        public GenericRepository<EFDocuments> DocumentsRepository
            {
            get
                {
                if (this.documentsRepository == null)
                    this.documentsRepository = new GenericRepository<EFDocuments>(context);
                return documentsRepository;
                }
            }

        public void Save()
            {
            try
                {
                context.SaveChanges();
                }
            catch (DbEntityValidationException e)
                {

                var outputLines = new List<string>();
                foreach (var eve in e.EntityValidationErrors)
                    {
                    outputLines.Add(string.Format("{0}: Entity of type \"{1}\" in state \"{2}\" has the following validation errors:", DateTime.Now, eve.Entry.Entity.GetType().Name, eve.Entry.State));
                    foreach (var ve in eve.ValidationErrors)
                        {
                        outputLines.Add(string.Format("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage));
                        }
                    }
                System.IO.File.AppendAllLines(@"E:\errors.txt", outputLines);
                throw e;
                }
            }

        private bool disposed = false;
        /// <summary>
        /// Protected Virtual Dispose method
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
            {
            if (!this.disposed)
                {
                if (disposing)
                    {
                    Debug.WriteLine("UnitOfWork is being disposed");
                    context.Dispose();
                    }
                }
            this.disposed = true;
            }

        /// <summary>
        /// Dispose method
        /// </summary>
        public void Dispose()
            {
            Dispose(true);
            GC.SuppressFinalize(this);
            }
        }
    }
