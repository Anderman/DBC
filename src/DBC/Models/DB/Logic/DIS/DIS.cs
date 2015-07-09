using GGZDBC.Models.DBCModel.Registraties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace GGZDBC.ModelsLogic.DIS
{
    /*
    DIS: All validated DBC are selectable for DIS sending. changing after sending leeds to delete record and insert record???
    Dis will check every DBC and can send a message back if invalid
    A dis zip file will contain a number of files en a special header etc
    Dis file will be send by PVM?
    DIs response by email and visable on www.disportal.nl site??
    Dis file is complete good or fault. So we need something to reset the status of all DBC send to DIS. 
    Problem: if another DIS files already send. So we need a check that the last DIS file was valited. mandatory check or a automated email check
    Delete and Insert of 1 DBC not allowed in one DIS file. 
    Patient change (Delete not allowed)
    Zorgtraject change is resent
    DBC change is resent
    ZP change is resent
    Functionality needed
    * Detect change in Patient record  (not needed always send if something changed in DBC or ZT)
    * Detect change/delete in Zorgtraject record (not needed??? startdate, Diagnose, diagnosedate)
    * Detect DBC delete/change
    Solution: 
    * All send DBC's are logged,
    * All DBC has a DISchangedate. (Closed DBC can't change anything related to dbc. so we can keep a changedate record)
    * If changedate is changed after Location.DISLastsuccesfullDeliveryDate
    * Deleted record will first have a dummy insert record in first file. And in second file a delete record
    * Batch program every month
    * for each agbcode
        for each DISlocationcode
            create a insertDeletezipfile with
                foreach patient with changeDBC
    Create a form for http://www.cloudmailin.com/plans
    * receive email and check if succesfull and set last senddate for agbcode and location
    */
    public class DIS
    {
        public List<DBCs> DBCs;
        public void Export()
        {
            try
            {
                foreach (DBCs DBC in SelectDBCforExport())
                {
                    CopyDBCFieldToDIS();
                }
                CreateDISZipfile();
                SendDISZipFIle();
            }
            catch (Exception ex)
            {
                //Log.ErrorFormat("Error creating ZIPFile:{0}", ex.ToString());
                throw;
            }
        }

        private void CreateDISZipfile()
        {
            throw new NotImplementedException();
        }

        private void SendDISZipFIle()
        {
            throw new NotImplementedException();
        }

        private void CopyDBCFieldToDIS()
        {
            throw new NotImplementedException();
        }

        private IEnumerable<DBCs> SelectDBCforExport()
        {
            throw new NotImplementedException();
        }
        public void ReExport(string OldDIS)
        {
        }


    }
}