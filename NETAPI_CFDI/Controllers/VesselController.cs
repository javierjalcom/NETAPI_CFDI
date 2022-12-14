using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;


using System.Data.Odbc;
using System.Data.OleDb;
using System.Security.Cryptography;
using System.Data;


using Newtonsoft.Json;
using System.Text;


namespace NETAPI_CFDI.Controllers
{
    [RoutePrefix("api/Vessel")]
    public class VesselController : ApiController
    {
        // GET: api/Vessel
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        //// GET: api/Vessel/5
        //public string Get(int id)
        //{
        //    return "value";
        //}




        //// POST: api/Vessel
        //public void Post([FromBody]string value)
        //{
        //}

        //// PUT: api/Vessel/5
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE: api/Vessel/5
        //public void Delete(int id)
        //{
        //}

        [HttpGet]
        [Route("Liquidation")]
        //public string Liquidation([FromUri] ContSearchParam aobj_Param)
        public HttpResponseMessage Liquidation([FromUri] long lngFolioId)
        {
            

            DataTable ldtb_Result = new DataTable();// ' la tabla que obtiene el resultado
            OleDbDataAdapter iAdapt_comand = new OleDbDataAdapter();
            OleDbCommand iolecmd_comand = new OleDbCommand();
            OleDbConnection ioleconx_conexion = new OleDbConnection();// '' objeto de conexion que se usara para conectar
                                                                      //ADODB.conection obj = new ADODB.conection();

           

            var response = this.Request.CreateResponse(HttpStatusCode.OK);
  

            int valueint = 0;
            string istr_conx = "";// ' cadena de conexion
            string strSQL = "";
            string lstr_data = "";
            
            istr_conx = System.Configuration.ConfigurationManager.ConnectionStrings["dbCalathus"].ToString();
            ioleconx_conexion.ConnectionString = istr_conx;
            iolecmd_comand = ioleconx_conexion.CreateCommand();

            ldtb_Result = new DataTable("User");
            strSQL = "spWEBLiquidation";

            iolecmd_comand.Parameters.Add("aintFolioId", OleDbType.Numeric);
            iolecmd_comand.Parameters["aintFolioId"].Value = lngFolioId;

            iolecmd_comand.CommandText = strSQL;
            iolecmd_comand.CommandType = CommandType.StoredProcedure;
            iolecmd_comand.CommandTimeout = 99999;
            List<ItemLiquidation> lobj_listLiq = new List<ItemLiquidation>();
            List<MasterLiquidation> lobj_masteliq = new List<MasterLiquidation>();
            ItemLiquidation lobj_itemliq;
            MasterLiquidation lobj_masterliqitem;

            ClsVoyage lobj_Voyage = new ClsVoyage();

            LiquidationInfo retuobject = new LiquidationInfo();

            retuobject.status = "OK";
            retuobject.voyage = lobj_Voyage;            
            retuobject.groupResume = lobj_masteliq;



            try
            {
                int lvalueint = 0;
                decimal lvaluedecimal = 0;

                int lint_GroupCount = 0;
                int lint_GroupId = 0;
                int lint_rowlimit = 0;
                int lint_rowindex = 0;
                int lint_itemsumvalue = 0;
                int lint_hastoinsert = 0;

                iAdapt_comand.SelectCommand = iolecmd_comand;
                iAdapt_comand.Fill(ldtb_Result);

                if (ldtb_Result.Rows.Count > 0 && ldtb_Result.Columns.Count > 1)
                {
                    lobj_masterliqitem = new MasterLiquidation();
                    lint_rowlimit = ldtb_Result.Rows.Count ;
                    /// master voyage
                    lobj_Voyage.calCode = ldtb_Result.Rows[0]["strCalCode"].ToString();
                    lobj_Voyage.impoId = ldtb_Result.Rows[0]["strimpoId"].ToString();
                    lobj_Voyage.expoId = ldtb_Result.Rows[0]["strexpoId"].ToString();

                    lobj_Voyage.voyageDate = ldtb_Result.Rows[0]["strVoyageDate"].ToString();
                    lobj_Voyage.comments = "";
                    lobj_Voyage.vesselCode = ldtb_Result.Rows[0]["strvesselCode"].ToString();
                    lobj_Voyage.vesselName = ldtb_Result.Rows[0]["strvesselName"].ToString();
                    lobj_Voyage.vesselCountry = ldtb_Result.Rows[0]["strvesselCountry"].ToString();

                   

                    lvalueint = 0;
                    if (int.TryParse(ldtb_Result.Rows[0]["intCountSumItem"].ToString(), out lvalueint) == false)
                        lvalueint = 0;

                    lint_GroupCount = lvalueint;

                   // objetos del master principal 

                   retuobject.calref = ldtb_Result.Rows[0]["strcalref"].ToString();
                    retuobject.operDate = ldtb_Result.Rows[0]["stroperDate"].ToString();
                    retuobject.type = ldtb_Result.Rows[0]["strtype"].ToString();

                    // ordenamiento de resultados 
                    DataView dv = new DataView(ldtb_Result);
                    dv.Sort = "intCountSumItem ASC";
                    ldtb_Result = dv.ToTable();

                    
                    //////////////

                    lint_rowindex = 0;
                    lint_itemsumvalue = 0;
                    lint_hastoinsert = 0;
                    /// recorrido de items
                    while (lint_rowindex < lint_rowlimit)
                    {
                        // obtener el numero de id , del renglon 
                        lvalueint = 0;
                        if (int.TryParse(ldtb_Result.Rows[lint_rowindex]["intVesselOpSumItemId"].ToString(), out lvalueint) == false)
                            lvalueint = 0;

                        // si es diferente crear nuevo grupo
                        if (lint_itemsumvalue != lvalueint )
                        {
                            lint_hastoinsert = 1;
                            lint_itemsumvalue = lvalueint;
                        }

                      
                        lint_GroupCount = lvalueint;

                        // ver si se inserta el master 
                        if (lint_hastoinsert ==1)
                        {

                            // leer informacion del master 
                            // objetos del master secundario
                            // nuevo ojvetos

                            lobj_masterliqitem = new MasterLiquidation();
                            lobj_masterliqitem.itemCode = ldtb_Result.Rows[lint_rowindex]["stritemCode"].ToString();

                            //
                            lvalueint = 0;
                            if (int.TryParse(ldtb_Result.Rows[lint_rowindex]["strintsize"].ToString(), out lvalueint) == false)
                                lvalueint = 0;

                            if (lvalueint > 0)
                                lobj_masterliqitem.size = lvalueint;
                            else
                                lobj_masterliqitem.size = 0;

                            //
                            lvalueint = 0;
                            if (int.TryParse(ldtb_Result.Rows[lint_rowindex]["intFull"].ToString(), out lvalueint) == false)
                                lvalueint = 0;

                            if (lvalueint > 0)
                                lobj_masterliqitem.full = true;
                            else
                                lobj_masterliqitem.full = false;

                            ///
                            lvaluedecimal = 0;
                            if (decimal.TryParse(ldtb_Result.Rows[lint_rowindex]["decweight"].ToString(), out lvaluedecimal) == false)
                                lvaluedecimal = 0;

                            lobj_masterliqitem.weight = lvaluedecimal;

                            ///
                            lvaluedecimal = 0;
                            if (decimal.TryParse(ldtb_Result.Rows[lint_rowindex]["decpct"].ToString(), out lvaluedecimal) == false)
                                lvaluedecimal = 0;

                            lobj_masterliqitem.pct = lvaluedecimal;

                            //
                            lvalueint = 0;
                            if (int.TryParse(ldtb_Result.Rows[lint_rowindex]["intquantity"].ToString(), out lvalueint) == false)
                                lvalueint = 0;

                            lobj_masterliqitem.quantity = lvalueint;

                            //
                            lvalueint = 0;
                            if (int.TryParse(ldtb_Result.Rows[lint_rowindex]["blnintimo"].ToString(), out lvalueint) == false)
                                lvalueint = 0;

                            if (lvalueint > 0)
                                lobj_masterliqitem.imo = true;
                            else
                                lobj_masterliqitem.imo = false;
                            ///

                            //
                            lvalueint = 0;
                            if (int.TryParse(ldtb_Result.Rows[lint_rowindex]["blintsdim"].ToString(), out lvalueint) == false)
                                lvalueint = 0;

                            if (lvalueint > 0)
                                lobj_masterliqitem.sdim = true;
                            else
                                lobj_masterliqitem.sdim = false;
                            ///

                            //
                            lvalueint = 0;
                            if (int.TryParse(ldtb_Result.Rows[lint_rowindex]["blnintdomfest"].ToString(), out lvalueint) == false)
                                lvalueint = 0;

                            if (lvalueint > 0)
                                lobj_masterliqitem.domfest = true;
                            else
                                lobj_masterliqitem.domfest = false;
                            ///

                            //
                            lvalueint = 0;
                            if (int.TryParse(ldtb_Result.Rows[lint_rowindex]["blnintatach"].ToString(), out lvalueint) == false)
                                lvalueint = 0;

                            if (lvalueint > 0)
                                lobj_masterliqitem.atado = true;
                            else
                                lobj_masterliqitem.atado = false;
                            ///

                            //
                            lvalueint = 0;
                            if (int.TryParse(ldtb_Result.Rows[lint_rowindex]["blninttrans"].ToString(), out lvalueint) == false)
                                lvalueint = 0;

                            if (lvalueint > 0)
                                lobj_masterliqitem.trans = true;
                            else
                                lobj_masterliqitem.trans = false;
                            ///

                            //
                            lvalueint = 0;
                            if (int.TryParse(ldtb_Result.Rows[lint_rowindex]["blintconven"].ToString(), out lvalueint) == false)
                                lvalueint = 0;

                            if (lvalueint > 0)
                                lobj_masterliqitem.conven = true;
                            else
                                lobj_masterliqitem.conven = false;
                            ///
                            //
                            lvalueint = 0;
                            if (int.TryParse(ldtb_Result.Rows[lint_rowindex]["blintMovSlow"].ToString(), out lvalueint) == false)
                                lvalueint = 0;

                            if (lvalueint > 0)
                                lobj_masterliqitem.maniobraLenta = true;
                            else
                                lobj_masterliqitem.maniobraLenta = false;

                            /// // nuuevaa lista
                            lobj_masterliqitem.detail = new List<ItemLiquidation>();

                            /////
                            
                        }
                        ///////

                        // nuevo item cont
                        lobj_itemliq = new ItemLiquidation();

                        // leer info item
                        //
                        lvalueint = 0;
                        if (int.TryParse(ldtb_Result.Rows[lint_rowindex]["intreference"].ToString(), out lvalueint) == false)
                            lvalueint = 0;
                        //
                        lobj_itemliq.reference = lvalueint;
                        lobj_itemliq.universal = ldtb_Result.Rows[lint_rowindex]["struniversalid"].ToString();
                        lobj_itemliq.containerNO = ldtb_Result.Rows[lint_rowindex]["strcontainerNO"].ToString();

                        ///

                        ///
                        lvaluedecimal = 0;
                        if (decimal.TryParse(ldtb_Result.Rows[lint_rowindex]["decitemweight"].ToString(), out lvaluedecimal) == false)
                            lvaluedecimal = 0;

                        lobj_itemliq.weight = lvaluedecimal;

                        //


                        lvalueint = 0;
                        if (int.TryParse(ldtb_Result.Rows[lint_rowindex]["intitemquantity"].ToString(), out lvalueint) == false)
                            lvalueint = 0;

                        lobj_itemliq.quantity = lvalueint;

                        ///

                        lvalueint = 0;
                        if (int.TryParse(ldtb_Result.Rows[lint_rowindex]["intDocType"].ToString(), out lvalueint) == false)
                            lvalueint = 0;

                        lobj_itemliq.docType = lvalueint;
                        lobj_itemliq.vlvalue = ldtb_Result.Rows[lint_rowindex]["strBlValue"].ToString();
                      
                        ///////////// info item

                        // insertar en el detall sub 
                        lobj_masterliqitem.detail.Add(lobj_itemliq);



                        // ver asociar al submaster
                        ///  si se tiene que asociar el submaster al master
                        if (lint_hastoinsert == 1)
                        {                            
                            retuobject.groupResume.Add(lobj_masterliqitem);

                            // marcar que ya se inserto
                            lint_hastoinsert = 0;
                        }
                        
                        /// incremento
                        lint_rowindex++;

                    }// WHILE DE RECORRIDO

                    
                    /*
                    // hacer un ciclo de la cantidad de 
                       // agregar un for each para cada item 
                    foreach (DataRow lrow in ldtb_Result.Rows)
                    {
                        lobj_itemliq = new ItemLiquidation();
                         

                        //
                        lvalueint = 0;
                        if (int.TryParse(lrow["intreference"].ToString(), out lvalueint) == false)
                            lvalueint = 0;
                        //
                        lobj_itemliq.reference = lvalueint;
                        lobj_itemliq.universal = lrow["struniversalid"].ToString();
                        lobj_itemliq.containerNO = lrow["strcontainerNO"].ToString();

                        ///

                        ///
                        lvaluedecimal = 0;
                        if (decimal.TryParse(lrow["decitemweight"].ToString(), out lvaluedecimal) == false)
                            lvaluedecimal = 0;

                        lobj_itemliq.weight = lvaluedecimal;

                        //


                        lvalueint = 0;
                        if (int.TryParse(lrow["intitemquantity"].ToString(), out lvalueint) == false)
                            lvalueint = 0;

                        lobj_itemliq.quantity = lvalueint;

                        ///

                        lvalueint = 0;
                        if (int.TryParse(lrow["intDocType"].ToString(), out lvalueint) == false)
                            lvalueint = 0;

                        lobj_itemliq.docType = lvalueint;
                        lobj_itemliq.vlvalue = lrow["strBlValue"].ToString();

                        lobj_masterliqitem.detail.Add(lobj_itemliq);


                    } // for each
                    */

                  //  retuobject.groupResume.Add(lobj_masterliqitem);

                    retuobject.voyage = lobj_Voyage;
                    retuobject.groupResume = lobj_masteliq;

                }/// si hay mas de 1 columna y minimo un renglon 

                if (ldtb_Result.Columns.Count ==0 )
                {
                    response = this.Request.CreateResponse(HttpStatusCode.NotFound);
                    retuobject.status = "NotFound";
                }

                if (ldtb_Result.Columns.Count == 1 && ldtb_Result.Rows.Count==1 )
                {
                    response = this.Request.CreateResponse(HttpStatusCode.NotFound);
                    retuobject.status = "NotFound";
                }

            }
            catch (Exception ex)
            {
                string strError = ObtenerError(ex.Message, 99999);
                strError = ex.Message;


                response = this.Request.CreateResponse(HttpStatusCode.BadRequest);
                retuobject.status = "BadRequest";
                //if (strError != null)
                //    return strError;
                //else
                //    return "0";
                //return Dt_RetrieveErrorTable(strError);
            }
            finally
            {
                ioleconx_conexion.Close();
            }
            // return ldtb_Result;
            //return JsonConvert.SerializeObject(retuobject);

            string ljson = JsonConvert.SerializeObject(retuobject);

            //var response = this.Request.CreateResponse();

            //response.Content = new StringContent(ljson, Encoding.UTF8, "application/json");
            response.Content = new StringContent(ljson, Encoding.UTF8, "application/json");
            return response;
            

        }


        public string ObtenerError(String cad, int ex)
        {

            if ((cad.Contains(ex.ToString()) == true) && (cad.Contains("Sybase Provider]") == true))
            {

                int idx = cad.LastIndexOf("]");
                idx = idx + 1;

                if ((idx > 0) && (idx <= cad.Length))
                    return cad.Substring(idx);
                else
                    return "";

            }
            else
            {
                if (cad.Contains("SSybase Provider]") == true)
                {
                    int idx;
                    idx = cad.LastIndexOf("]");
                    idx = idx + 1;

                    if (idx > 0 && idx <= cad.Length)
                        return cad.Substring(idx);
                    else
                        return "";
                }

            } // else if((cad.Contains(ex.ToString()) == True) &&(cad.Contains("Sybase Provider]") == True))

            return "";

        }


        public class LiquidationInfo
        {
            public ClsVoyage voyage { get; set; }
            public string calref { get; set; }
            public string operDate { get; set; }
          //  public string customerSolicitante { get; set; }
         //   public string customerSATCP { get; set; }
         //   public string customerFacturacion { get; set; }
            public string type { get; set; }
            public string status { get; set; }
            public IList<MasterLiquidation> groupResume { get; set; }
        }

        public class ClsVoyage
        {
            public string calCode { get; set; }
            public string impoId { get; set; }
            public string expoId { get; set; }
            public string voyageDate { get; set; }
            public string comments { get; set; }
            public string vesselCode { get; set; }
            public string vesselName { get; set; }
            public string vesselCountry { get; set; }
            
            
        }
        public class MasterLiquidation
        {
            public string itemCode { get; set; }
            public int size { get; set; }
            public bool full { get; set; }
            public decimal weight { get; set; }
            public decimal pct { get; set; }
            public int quantity { get; set; }
            public bool imo { get; set; }
            public bool sdim { get; set; }
            public bool domfest { get; set; }
            public bool atado { get; set; }
            public bool trans { get; set; }
            public bool conven { get; set; }
            public bool maniobraLenta { get; set; }
            public IList<ItemLiquidation> detail { get; set; }

        }

        public class ItemLiquidation
        {
            public int reference { get; set; }
            public string universal { get; set; }
            public string containerNO { get; set; }
            public decimal weight { get; set; }
            public int quantity { get; set; }
            public int docType { get; set; }
            public string vlvalue { get; set; }
            
        }

   


    }
}
