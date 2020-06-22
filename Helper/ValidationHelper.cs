using Quant_BackTest_Backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Quant_BackTest_Backend.Helper {
    public class ValidationHelper {
        public static void safeSaveChanges(quantEntities ctx) {
            try {
                ctx.SaveChanges();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException e) {
                foreach (var eve in e.EntityValidationErrors) {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors) {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
            catch (Exception e) {
                throw;
            }
        }
    }
}