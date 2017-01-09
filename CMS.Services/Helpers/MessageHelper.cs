using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Services.Helpers
    {
    public interface IMessageHelper
        {
        /// <summary>
        /// If an entity with provided Id not found message
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        string GetNotFoundMessage(string entity);

        /// <summary>
        /// For a field which is required message
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        string GetRequiredMessage(string field);

        /// <summary>
        /// If a menuitem is not available for the order message
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        string GetNotAvailableMessage(string item);

        /// <summary>
        /// Message for an order that has pending items and cannot be cancelled
        /// </summary>
        /// <returns></returns>
        string GetCannotCancelOrderMessage();

        /// <summary>
        /// Message for a menuitem whose quantity cannot be zero
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        string GetCannotBeZeroMessage(string column);

        /// <summary>
        /// Message for an integer value that cannot be zero or less for a field
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        string GetLessThanZeroMessage(string column);

        /// <summary>
        /// Message for a numeric value which is not valid for a field
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        string GetInvalidNumericMessaage(string column);
        }

    public class MessageHelper : IMessageHelper
        {
        private string notFound = "";
        private string isRequired = "";
        private string notAvailableMessage = "";
        private string cannotCancelOrder = "";
        private string cannotBeZero = "";
        private string cannotBeLessThanZero = "";
        private string invalidNumeric = "";

        public MessageHelper()
            {
            this.notFound = " with provided id not found.";
            this.isRequired = " is required.";
            this.notAvailableMessage = " is not available";
            this.cannotBeZero = " cannot be less than or equal to zero.";
            this.cannotBeLessThanZero = " cannot be less than zero.";
            this.invalidNumeric = " must be a valid numeric value";
            }

        public string GetNotFoundMessage(string entity)
            {
            return (entity + this.notFound);
            }

        public string GetRequiredMessage(string field)
            {
            return (field + this.isRequired);
            }

        public string GetNotAvailableMessage(string item)
            {
            return (item + this.notAvailableMessage);
            }

        public string GetCannotCancelOrderMessage()
            {
            return this.cannotCancelOrder;
            }

        public string GetCannotBeZeroMessage(string column)
            {
            return (column + this.cannotBeZero);
            }

        public string GetLessThanZeroMessage(string column)
            {
            return (column + this.cannotBeLessThanZero);
            }

        public string GetInvalidNumericMessaage(string column)
            {
            return column + this.invalidNumeric;
            }
        }
    }
