using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows.Forms;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;
using System.Windows.Forms.DataVisualization.Charting;
using System.Net.Http;
using System.Net;

namespace Emmerre_Admin
{
    class EmmerreAdmin
    {
        private string ipaddress;
        private string username;
        private string userpass;
        private CookieAwareWebClient webclient = new CookieAwareWebClient();
        //Array di elementi
        public string[] Campagne = new string[150];//Campagne ID and Name STD;Campagna unica
        public string[] CampagneDettaglio = new string[150];//campagnaID;campagnaNome;campagnaStato;campagnaDialMode;
        public string[] CampagnaSettings = new string[150];//jsondata.Split(",".ToCharArray());  MOLTO DETTAGLIATO
        public Series PointSeries = new Series();//SERIE PUNTI GRAFICO
        public string[] ReportAgentDetail = new string[150]; //GetAgentStatusDetail();
        public string[] Lists = new string[200];//ListID;ListName;ListStatus;ListLastCall;ListLenght;ListCampaign
        public string ListDetail;
        public string[] ExcelFirstRow = new string[30];
        public string[] Agents = new string[400];
        public string[] AgentDetails;
        public string[] RealTimeAgentsStats = new string[1000];
        public string[] RealTimeAgentsStatsDetail = new string[500];
        public string[] AgentGroups = new string[50];
        public string[] SearchResults = new string[150];
        //Status Agenti vars Block
        public string AgentiInChiamata;
        public string AgentiInPausa;
        public string AgentiInAttesa;
        public string AgentiInLinea;
        public string[] UserInfoResults = new string[500];
        public string[] AgentTalkTime = new string[1000];
        public string[] AgentLoginLogout = new string[1000];
        public string[] AgentOutboundCalls = new string[1000];
        public string[] AgentManualDialCalls = new string[1000];
        public string[] AgentRecs = new string[1000];
        //CallStatus vars Block
        public string ChiamateInAttesa;
        public string CodaChiamateInUscita;
        public string CodaChiamateInEntrata;
        public string ChiamateInEntrata;
        public string ChiamateInUscita;
        public string ChiamateTotali;
        public string ChiamateRisposte;
        public string NumeriDisponibiliDaChiamare;
        public string NumeriTotaliTutteListe;
        public string PercentualeSaltate;
        public string ChiamateSaltate;
        //Server Info
        public string Hostname;
        public string ListeningIP;
        public string KernelVersion;
        public string DistroName;
        public string Uptime;
        public string LoadAverages;
        public string PhysicalMemoryUsed;
        public string TotalDiskUsage;
        //Sippy Info
        public string Balance;
        //Vendite Giornaliere
        public string VenditeTotali;
        public string VenditeInUscita;
        public string VenditeInIngresso;
        //Variabili Liste
        public string LastListId;
        public string[] ListEsitiArr;




        //Costruttore
        /// <summary>
        /// Crea nuovo oggetto.
        /// </summary>
        /// <param name="_ip">Indirizzo del server</param>
        /// <param name="_user">Username</param>
        /// <param name="_pass">Password</param>
        public EmmerreAdmin(string _ip, string _user, string _pass)
        {
            ipaddress = _ip;
            username = _user;
            userpass = _pass;
        }


        //Function to Login
        public void login()
        {
            webclient.BaseAddress = @"https://" + ipaddress;
            // establish login data
            var loginData = new NameValueCollection();
            loginData.Add("user_name", username);
            loginData.Add("user_pass", userpass);
            // begin login
            webclient.UploadValues("/index.php/go_login/validate_credentials", "POST", loginData);
        }
        //Get VEndite
        //VenditeTotali; VenditeInIngresso; VenditeInUscita
        public void GetSalesToday()
        {
            string source = webclient.DownloadString("https://" + ipaddress + "/index.php/go_site/go_dashboard_sales_today").Replace("\r", "").Replace("\t", "");
            string[] sourceArray = source.Split("\n".ToCharArray());
            VenditeTotali = sourceArray[5].Replace("<td class=\"b\"><a class=\"cur_hand\">", "").Replace("</a></td>", "");
            VenditeInIngresso = sourceArray[9].Replace("<td class=\"c\"><a class=\"cur_hand\">", "").Replace("</a></td>", "");
            VenditeInUscita = sourceArray[13].Replace("<td class=\"c\"><a class=\"cur_hand\">", "").Replace("</a></td>", "");
        }
        //Get info to calls
        public void GetStatistiche()
        {
            string source = webclient.DownloadString("https://" + ipaddress + "/index.php/go_site/go_dashboard_calls_today").Replace("\t", "");
            //AgentiInChiamata
            string[] sourcearray = source.Split("\n".ToCharArray());
            ChiamateTotali = sourcearray[63].Replace("<td class=\"c dropTD\"><div class=\"tdcon1\"><div class=\"tdcon2\"><a class=\"toolTip\" style=\"cursor:pointer\" onclick=\"callMonitoring()\" title=\"Click to see calls being placed\">", "").Replace("</a></div></div></td>", "");
            ChiamateInAttesa = sourcearray[20].Replace("<td class=\"o dropTD\"><div class=\"tdcon1\"><div class=\"tdcon2\"><a class=\"toolTip\" style=\"cursor:pointer\" onclick=\"callMonitoring()\" title=\"Click to see calls being placed\">", "").Replace("</a></div></div></td>", "");
            CodaChiamateInUscita = sourcearray[27].Replace("<td class=\"c dropTD\"><div class=\"tdcon1\"><div class=\"tdcon2\"><a class=\"toolTip\" style=\"cursor:pointer\" onclick=\"callMonitoring()\" title=\"Click to see calls being placed\">", "").Replace("</a></div></div></td>", "");
            CodaChiamateInEntrata = sourcearray[36].Replace("<td class=\"c dropTD\"><div class=\"tdcon1\"><div class=\"tdcon2\"><a class=\"toolTip\" style=\"cursor:pointer;\" onclick=\"callMonitoring()\" title=\"Click to see calls being placed\">", "").Replace("</a></div></div></td>", "");
            ChiamateInEntrata = sourcearray[44].Replace("<td class=\"c dropTD\"><div class=\"tdcon1\"><div class=\"tdcon2\"><a class=\"toolTip\" style=\"cursor:pointer\" onclick=\"callMonitoring()\" title=\"Click to see calls being placed\">", "").Replace("</a></div></div></td>", "");
            ChiamateInUscita = sourcearray[51].Replace("<td class=\"c dropTD\"><div class=\"tdcon1\"><div class=\"tdcon2\"><a class=\"toolTip\" style=\"cursor:pointer\" onclick=\"callMonitoring()\" title=\"Click to see calls being placed\">", "").Replace("</a></div></div></td>", "");
            //Numeritotali
            string source2 = webclient.DownloadString("https://" + ipaddress + "/index.php/go_site/go_dashboard_leads").Replace("\t", "");
            string[] sourcearray2 = source2.Split("\n".ToCharArray());
            NumeriDisponibiliDaChiamare = sourcearray2[37].Replace("<td class=\"c\"><a class=\"cur_hand\">", "").Replace("</a></td>", "");
            NumeriTotaliTutteListe = sourcearray2[41].Replace("<td class=\"c\"><a class=\"cur_hand\">", "").Replace("</a></td>", "");
            //Chiamaterispo e percentuale
            string source3 = webclient.DownloadString("https://" + ipaddress + "/index.php/go_site/go_dashboard_drops_today").Replace("\t", "");
            string[] sourcearray3 = source3.Split("\n".ToCharArray());
            PercentualeSaltate = sourcearray3[20].Replace("<td class=\"o dropTD\"><div class=\"tdcon1\"><div class=\"tdcon2\"><a class=\"toolTip\" style=\"cursor:pointer;font-size:50px;\" onclick=\"droppedCalls()\" title=\"Click to see the list of campaign dropped percentage\">", "").Replace("</a></div></div></td>", "");
            ChiamateSaltate = sourcearray3[28].Replace("<td class=\"c dropTD\"><div class=\"tdcon1\"><div class=\"tdcon2\"><a class=\"toolTip\" style=\"cursor:pointer\" onclick=\"droppedCalls()\" title=\"Click to see the list of campaign dropped calls\">", "").Replace("</a></div></div></td>", "");
            ChiamateRisposte = sourcearray3[36].Replace("<td class=\"c dropTD\"><div class=\"tdcon1\"><div class=\"tdcon2\"><a class=\"toolTip\" style=\"cursor:pointer\" onclick=\"droppedCalls()\" title=\"Click to see the list of campaign answered calls\">", "").Replace("</a></div></div></td>", "");
        }
        //Get SippyInfo
        public void GetSippyInfo()
        {
            string source = webclient.DownloadString("https://" + ipaddress + "/index.php/go_site/sippyinfo");
            int startindex = source.IndexOf("payWithPayPalbalance");
            int bal0 = source.IndexOf("</a>", startindex);
            int bal1 = source.LastIndexOf("\">", bal0);
            Balance = source.Substring(bal1 + 2, bal0 - (bal1 + 2));
        }
        //Return Points Series for Chart
        public void GetChartData(string daData, string aData, string campagna)
        {
            try
            {
                //Get Points Data
                webclient.DownloadString("https://" + ipaddress + "/index.php/go_site/go_reports_output/stats/" + daData + "/" + aData + "/" + campagna + "/daily/");
                //Get Points Data JSON
                string pointsData = webclient.DownloadString("https://" + ipaddress + "/data/stats-daily-ADMIN.json");
                int index1 = pointsData.IndexOf("[[");
                int index2 = pointsData.IndexOf("]]");
                pointsData = pointsData.Substring(index1 + 2, index2 - (index1 + 2)).Replace("],[", "\n").Replace("\"", "");
                string[] PointsArray = pointsData.Split("\n".ToCharArray());
                //Settings DataPointsCollection
                for (int i = 0; i < PointsArray.Length; i++)
                {
                    string[] values = PointsArray[i].Split(",".ToCharArray());
                    PointSeries.Points.AddXY(Convert.ToInt32(values[0]), Convert.ToInt32(values[1]));
                }
            }
            catch (Exception) { return; }
        }





        //#######################################################################################################################################
        //##############################################        GESTIONE AGENTI             #####################################################
        //Function to get Agent Status
        //AgentiInChiamata; AgentiInPausa; AgentiInAttesa; AgentiInLinea
        public void GetAgentStatus()
        {
            string source = webclient.DownloadString("https://" + ipaddress + "/index.php/go_site/go_dashboard_agents").Replace("\t", "");
            //AgentiInChiamata
            string[] sourcearray = source.Split("\n".ToCharArray());
            AgentiInChiamata = sourcearray[5].Replace("<td class=\"b\"><a class=\"cur_hand toolTip\" style=\"cursor:pointer\" onclick=\"agentMonitoring()\" title=\"Click to monitor agents\">", "").Replace("</a></td>", "");
            AgentiInPausa = sourcearray[9].Replace("<td class=\"c\"><a class=\"cur_hand toolTip\" style=\"cursor:pointer\" onclick=\"agentMonitoring()\" title=\"Click to monitor agents\">", "").Replace("</a></td>", "");
            AgentiInAttesa = sourcearray[13].Replace("<td class=\"c\"><a class=\"cur_hand toolTip\" style=\"cursor:pointer\" onclick=\"agentMonitoring()\" title=\"Click to monitor agents\">", "").Replace("</a></td>", "");
            AgentiInLinea = sourcearray[17].Replace("<td class=\"b\"><a class=\"cur_hand toolTip\" style=\"cursor:pointer\" onclick=\"agentMonitoring()\" title=\"Click to monitor agents\">", "").Replace("</a></td>", "");
        }
        //Dettaglio Agenti
        //ReportAgentDetail[];
        public void GetAgentStatusDetail(string daData, string aData, string campagna)
        {
            string source = webclient.DownloadString("https://" + ipaddress + "/index.php/go_site/go_reports_output/agent_pdetail/" + daData + "/" + aData + "/" + campagna).Replace("\t","").Replace("\r","");
            //Check if there are results in here
            if (source.Contains("Nessun agente trovato in questo periodo"))
            {
                ReportAgentDetail[0] = "Nessun agente trovato in questo periodo";
                return;
            }

            int agentdetailStart = source.IndexOf("<!-- Start Agent Performance Detail -->");
            int agentdetailEnd = source.IndexOf("<!-- End Agent Performance Detail -->");
            string AgentDetail = source.Substring(agentdetailStart, agentdetailEnd - agentdetailStart);
            AgentDetail = AgentDetail.Remove(0, AgentDetail.IndexOf("<tr style=\"background-color:#E0F8E0;\">") + 38);
            AgentDetail = AgentDetail.Remove(AgentDetail.IndexOf("</table>")).Replace("</tr><tr style=\"background-color:#EFFBEF;\">","@").Replace("</tr><tr style=\"background-color:#E0F8E0;\">","@");
            AgentDetail = AgentDetail.Replace("        ", "").Replace("</tr>      <tr style=\"background-color:#FFFFFF;\">", "@").Replace("left","right");
            AgentDetail = AgentDetail.Replace("<td nowrap style=\"border-top:dashed 1px #D0D0D0;\"><div align=\"right\" class=\"style4\" style=\"font-size: 10px;\">&nbsp; ", ";").Replace(" &nbsp;</div></td>","");
            AgentDetail = AgentDetail.Replace("<td nowrap style=\"border-top:#D0D0D0 dashed 1px;\"><div align=\"right\" class=\"style4\" style=\"font-size:10px\"><b>TOTALE</b></div></td>", "");
            AgentDetail = AgentDetail.Replace("<td nowrap style=\"border-top:#D0D0D0 dashed 1px;\"><div align=\"right\" class=\"style4\" style=\"font-size:10px\">&nbsp; ", ";");
            AgentDetail = AgentDetail.Replace("</strong> ", "").Replace("<strong>", "").Replace("      </tr>","").Replace("\n","");
            string[] AgentArray = AgentDetail.Split("@".ToCharArray());
            for(int i = 0; i < AgentArray.Length; i++)
            {
                ReportAgentDetail[i] = AgentArray[i].Remove(0, 1);
            }
        }
        public void GetAgents()
        {
            int page = 1;
            int agentindex = 0;
            string source = webclient.DownloadString("https://" + ipaddress + "/index.php/go_user_ce/index/search/1").Replace("\t", "").Replace("\r", "");
            while (source.Contains("user-cols-container"))
            {
                //Get source for this page
                source = webclient.DownloadString("https://" + ipaddress + "/index.php/go_user_ce/index/search/" + page).Replace("\t", "").Replace("\r", "");
                //Modify Source
                int user0 = source.IndexOf("user-tbl-rows");
                int user1 = source.IndexOf("</tbody>", user0);
                source = source.Substring(user0 + 16, user1 - (user0 + 16));
                source = source.Replace("<td style=\"border-top:#D0D0D0 dashed 1px;\">&nbsp;&nbsp;", "");
                source = source.Replace("<a class='action-id toolTip' style='cursor:pointer' title='Modify user ", "");
                source = source.Replace("<span style=\"color:blu;font-weight:bold;\">", "").Replace("<tr style=\"background-color:#c3e1ff;\" class='user-tbl-rows'>","").Replace("<tr style=\"background-color:#65a2df;\" class='user-tbl-rows'>", "");
                //Get Text With only one agent
                for(int i = 0; source.IndexOf("user-action-modify-") > 0; i++)
                {
                    int agent0 = source.IndexOf(">");
                    int agent1 = source.IndexOf("</span></td>",agent0);
                    string data = source.Substring(agent0 + 1, agent1 - (agent0 + 1));
                    //Get Data
                    int agid1 = data.IndexOf("</a></td>");
                    string AgentId = data.Substring(0, agid1);
                    //Adjust remaining data of agent
                    data = data.Replace("</a></td>", ";").Replace("</td>", ";").Replace("\n","");
                    //Another step
                    int trim0 = data.IndexOf(">");
                    data = AgentId + ";" + data.Substring(trim0 + 1, data.Length - (trim0 + 1));
                    //ADD DATA TO ARRAY
                    Agents[agentindex] = data;
                    //Remove this agent from source
                    agent1 = source.IndexOf("</tr>");
                    source = source.Substring(agent1 + 5, source.Length - (agent1 + 5));
                    //Agent index increment
                    agentindex++;
                }
                page++;
                source = webclient.DownloadString("https://" + ipaddress + "/index.php/go_user_ce/index/search/" + page).Replace("\t", "").Replace("\r", "");
            }
        }//Tutti gli agenti in Agents[]   ID;nome;livello;gruppo;stato
        public void AgentsLogout(string CampaignID)
        {
            var reqparm = new System.Collections.Specialized.NameValueCollection();
            reqparm.Add("campaign", CampaignID);
            byte[] responsebytes = webclient.UploadValues("https://" + ipaddress + "/index.php/go_campaign_ce/emergencylogout", "POST", reqparm);
        }//Slogga tutti gli agenti in una campagna
        public void AgentLogout(string AgentID)
        {
            var reqparm = new System.Collections.Specialized.NameValueCollection();
            reqparm.Add("user", AgentID);
            byte[] responsebytes = webclient.UploadValues("https://" + ipaddress + "/index.php/go_user_ce/emergencylogout", "POST", reqparm);
        }//Slogga il singolo agente
        //Get Numeric agent ID from string ID
        public string GetAgentID(string agentName)
        {
            int page = 1;
            string agent = agentName;
            string source = webclient.DownloadString("https://" + ipaddress + "/index.php/go_user_ce/index/search/" + page + "/" + agent).Replace("\t", "").Replace("\r", "");
            //Check if agent was found in page 1
            while (!source.Contains("user-action-modify-"))
            {
                source = webclient.DownloadString("https://" + ipaddress + "/index.php/go_user_ce/index/search/" + page + "/" + agent).Replace("\t", "").Replace("\r", "");
                page++;
            }
            //Get Agent ID
            int id0 = source.IndexOf("rel='");
            int id1 = source.IndexOf("'", id0 + 6);
            string AgentID = source.Substring(id0 + 5, id1 - (id0 + 5));
            return AgentID;
        }
        //Get Agent group
        public string GetAgentGroup(string agentNameID)
        {
            GetAgentSettings(agentNameID);
            string gruppo = AgentDetails[5].Split(":".ToCharArray())[1];
            return gruppo;
        }
        public void GetAgentGroups()
        {
            webclient.Credentials = new NetworkCredential(username, userpass);
            string source = webclient.DownloadString("https://" + ipaddress + "/_vicidial_/realtime_report.php");
            int gro0 = source.IndexOf("var select_list = '");
            int gro1 = source.IndexOf(";",gro0);
            source = source.Substring(gro0 + 19, gro1 - (gro0 + 19));
            //Adjuctments
            gro0 = source.IndexOf("<SELECT SIZE=8 NAME=user_group_filter[] ID=user_group_filter[] multiple>");
            gro1 = source.IndexOf("</SELECT>",gro0 + 20);
            source = source.Substring(gro0 + 72, gro1 - (gro0 + 72));
            source = source.Replace("<option selected value=\"", "").Replace("</option><option value=\"",";").Replace("</option>","").Replace("\">",",");
            //Create array of group
            AgentGroups = source.Split(";".ToCharArray());
            AgentGroups = AgentGroups.Where(c => c != null).ToArray();
        }
        //Get Setting for specific agent
        public void GetAgentSettings(string agentID)
        {
            string ID = GetAgentID(agentID);
            string source = webclient.DownloadString("https://" + ipaddress + "/index.php/go_user_ce/collectuserinfo/" + ID);
            //Collect info
            source = source.Replace("}]", "").Replace("[{", "").Replace("\"","").Replace(",","\n");
            AgentDetails = source.Split("\n".ToCharArray());
        }
        //Cambia il nome dell'agente
        public void ChangeAgentName(string agentID,string newName)
        {
            //Get agent Settings
            GetAgentSettings(agentID);

            var reqparm = new System.Collections.Specialized.NameValueCollection();
            //VARS
            string pass = AgentDetails[2].Split(":".ToCharArray())[1];
            string ID = GetAgentID(agentID);
            string fullname = AgentDetails[3].Split(":".ToCharArray())[1];
            string phone_login = AgentDetails[6].Split(":".ToCharArray())[1];
            string phone_pass = AgentDetails[7].Split(":".ToCharArray())[1];
            string user_group = AgentDetails[5].Split(":".ToCharArray())[1];
            string active = AgentDetails[58].Split(":".ToCharArray())[1];
            string hotkeys_active = AgentDetails[20].Split(":".ToCharArray())[1];
            string user_level = AgentDetails[4].Split(":".ToCharArray())[1];
            string modify_same_user_level = AgentDetails[95].Split(":".ToCharArray())[1];
            //Set new name
            fullname = newName;
            //Set Params
            reqparm.Add("users_id-" + agentID, agentID);
            reqparm.Add("vicidial_user_id-" + agentID, ID);
            reqparm.Add(agentID + "-pass", pass);
            reqparm.Add(agentID + "-full_name", fullname);
            reqparm.Add("phone_login-" + agentID, phone_login);
            reqparm.Add("phone_pass-" + agentID, phone_pass);
            reqparm.Add("user_group-" + agentID, user_group);
            reqparm.Add("active-" + agentID, active);
            reqparm.Add("hotkeys_active-" + agentID, hotkeys_active);
            reqparm.Add("user_level-" + agentID, user_level);
            reqparm.Add("modify_same_user_level-" + agentID, modify_same_user_level);
            //Send Request
            byte[] responsebytes = webclient.UploadValues("https://" + ipaddress + "/index.php/go_user_ce/updateuser/" + ID + "/" + agentID, "POST", reqparm);
        }
        //Change agent password
        public void ChangeAgentPass(string agentID, string newpass)
        {
            //Get agent Settings
            GetAgentSettings(agentID);

            var reqparm = new System.Collections.Specialized.NameValueCollection();
            //VARS
            string pass = AgentDetails[2].Split(":".ToCharArray())[1];
            string ID = GetAgentID(agentID);
            string fullname = AgentDetails[3].Split(":".ToCharArray())[1];
            string phone_login = AgentDetails[6].Split(":".ToCharArray())[1];
            string phone_pass = AgentDetails[7].Split(":".ToCharArray())[1];
            string user_group = AgentDetails[5].Split(":".ToCharArray())[1];
            string active = AgentDetails[58].Split(":".ToCharArray())[1];
            string hotkeys_active = AgentDetails[20].Split(":".ToCharArray())[1];
            string user_level = AgentDetails[4].Split(":".ToCharArray())[1];
            string modify_same_user_level = AgentDetails[95].Split(":".ToCharArray())[1];
            //Set new pass
            pass = newpass;
            //Set Params
            reqparm.Add("users_id-" + agentID, agentID);
            reqparm.Add("vicidial_user_id-" + agentID, ID);
            reqparm.Add(agentID + "-pass", pass);
            reqparm.Add(agentID + "-full_name", fullname);
            reqparm.Add("phone_login-" + agentID, phone_login);
            reqparm.Add("phone_pass-" + agentID, phone_pass);
            reqparm.Add("user_group-" + agentID, user_group);
            reqparm.Add("active-" + agentID, active);
            reqparm.Add("hotkeys_active-" + agentID, hotkeys_active);
            reqparm.Add("user_level-" + agentID, user_level);
            reqparm.Add("modify_same_user_level-" + agentID, modify_same_user_level);
            //Send Request
            byte[] responsebytes = webclient.UploadValues("https://" + ipaddress + "/index.php/go_user_ce/updateuser/" + ID + "/" + agentID, "POST", reqparm);
        }
        public void GetRealTimeAgents(string _group,string _campaign)
        {
            string gruppo = _group;
            string campagna = _campaign;
            //Get REal Time Agents Data
            string source = webclient.DownloadString("https://" + ipaddress + "/index.php/go_site/go_monitoring/0/timeup/agents/" + gruppo + "/" + campagna);
            //Substring useful data
            int data0 = source.IndexOf("</th></tr></thead><tbody><tr style=\"color:#333\" align=center>");
            int data1 = source.IndexOf("</tr></tbody>");
            source = source.Substring(data0 + 62, data1 - (data0 + 63));
            //Adjust new data
            source = source.Replace(" id=\"trid\"", "").Replace("display: none;color:#333;", "color:#333").Replace("<td nowrap style=\"font-size:11px;\">&nbsp;", "").Replace("</span>&nbsp;","").Replace("&nbsp;","");
            source = source.Replace("<td nowrap style=\"font-size:11px;cursor:pointer;\" class=\"toolTip\" title=\"Click to listen or barge:<br />", "").Replace("\"><span id=\"sendMonitor\" onclick=\"sendMonitor('","").Replace("','" + ipaddress + "');\">","§");
            source = source.Replace("<td nowrap style=\"font-size:11px;background-color:", "").Replace(";color:black;\" >", ",").Replace(";color:white;\" >", ",");
            source = source.Replace("<span style=\"cursor:pointer\" onclick=\"modify('", "").Replace("')\">", ",").Replace("<span>","");
            source = source.Replace("</tr><tr style=\"color:#333\" align=center>", "@");
            source = source.Replace("  ","").Replace("\n","");
            //Put Data in to array
            string[] _realtimedata = source.Split("@".ToCharArray());
            //Ultimi aggiustamenti
            for (int i = 0; i < _realtimedata.Length; i++)
            {
                int uno = _realtimedata[i].IndexOf("§");
                int due = _realtimedata[i].Length;
                _realtimedata[i] = _realtimedata[i].Substring(uno + 1, due - (uno + 1));
            }
            //Finally Put DataS in Array
            RealTimeAgentsStats = _realtimedata;
        }
        public void GetRealTimeAgentsDetail(string campagna,string gruppo)
        {
            /*string auth = username + ":" + userpass;
            var authByte = System.Text.Encoding.UTF8.GetBytes(auth);
            auth = System.Convert.ToBase64String(authByte);
            webclient.Headers.Add("Authorization", "Basic " + auth);*/
            webclient.Credentials = new NetworkCredential(username, userpass);
            //#########
            var reqparm = new System.Collections.Specialized.NameValueCollection();
            //VARS
            string RTajax = "1";
            string DB = "0";
            string groups = "ALL-ACTIVE";
            string user_group_filter = "ALL-GROUPS";
            string adastats = "2";
            string SIPmonitorLINK = "";
            string IAXmonitorLINK = "";
            string usergroup = "";
            string UGdisplay = "0";
            string UidORname = "1";
            string orderby = "timeup";
            string SERVdisplay = "0";
            string CALLSdisplay = "1";
            string PHONEdisplay = "0";
            string CUSTPHONEdisplay = "0";
            string with_inbound = "Y";
            string monitor_active = "";
            string monitor_phone = "";
            string ALLINGROUPstats = "";
            string DROPINGROUPstats = "0";
            string NOLEADSalert = "";
            string CARRIERstats = "0";
            string PRESETstats = "0";
            string AGENTtimeSTATS = "0";
            //Settings Parameter
            groups = campagna;
            user_group_filter = gruppo;
            //Set Params
            reqparm.Add("RTajax", RTajax);
            reqparm.Add("DB", DB);
            reqparm.Add("groups[]", groups);
            reqparm.Add("user_group_filter[]", user_group_filter);
            reqparm.Add("adastats", adastats);
            reqparm.Add("SIPmonitorLINK", SIPmonitorLINK);
            reqparm.Add("IAXmonitorLINK", IAXmonitorLINK);
            reqparm.Add("usergroup", usergroup);
            reqparm.Add("UGdisplay", UGdisplay);
            reqparm.Add("UidORname", UidORname);
            reqparm.Add("orderby", orderby);
            reqparm.Add("SERVdisplay", SERVdisplay);
            reqparm.Add("CALLSdisplay", CALLSdisplay);
            reqparm.Add("PHONEdisplay", PHONEdisplay);
            reqparm.Add("CUSTPHONEdisplay", CUSTPHONEdisplay);
            reqparm.Add("with_inbound", with_inbound);
            reqparm.Add("monitor_active", monitor_active);
            reqparm.Add("monitor_phone", monitor_phone);
            reqparm.Add("ALLINGROUPstats", ALLINGROUPstats);
            reqparm.Add("DROPINGROUPstats", DROPINGROUPstats);
            reqparm.Add("NOLEADSalert", NOLEADSalert);
            reqparm.Add("CARRIERstats", CARRIERstats);
            reqparm.Add("PRESETstats", PRESETstats);
            reqparm.Add("AGENTtimeSTATS", AGENTtimeSTATS);
            //Send Request
            byte[] responsebytes = webclient.UploadValues("https://" + ipaddress + "/_vicidial_/AST_timeonVDADall.php", "POST", reqparm);
            //Decode response byte
            string source = new ASCIIEncoding().GetString(responsebytes);
            //Remove Unuseful data from source
            int _zero = source.IndexOf("+----------------+------------------------+-----------+-----------------+---------+------------+-------+------+------------------") + 5;
            int zero = source.IndexOf("+----------------+------------------------+-----------+-----------------+---------+------------+-------+------+------------------", _zero);
            int uno = source.IndexOf("+----------------+------------------------+-----------+-----------------+---------+------------+-------+------+------------------", zero + 10);
            //Get Clean String
            source = source.Substring(zero + 130, uno - (zero + 131));
            //Adjust Data
            string[] _Agenti = source.Split("\n".ToCharArray());
            for(int i = 0; i < _Agenti.Length; i++)
            {
                _Agenti[i] = _Agenti[i].Replace("      </B></SPAN> ", "").Replace(" <SPAN class=\"","").Replace("<SPAN class=\"","").Replace(" <a href=\"./user_status.php?user=","").Replace("\" target=\"_blank\">","|").Replace("\"><B>","|");
                _Agenti[i] = _Agenti[i].Replace("</B></SPAN></a> <a href=\"javascript:ingroup_info('", "|").Replace("','" + i + "');\">+</a>","").Replace("  </B></SPAN> ","").Replace(" </B></SPAN>          ","").Replace("</B></SPAN> ","");
                //Remove Blank chars
                _Agenti[i] = _Agenti[i].Replace("        ", "").Replace("       ","").Replace("    ","").Replace("</a> <a href=\"javascript:ingroup_info('","");
                _Agenti[i] = _Agenti[i].Substring(1, _Agenti[i].Length - 1);
            }
            //Put Results to Array
            RealTimeAgentsStatsDetail = _Agenti;
        }
        public void AscoltoAgente(string palmarino, string AgentSessionID)
        {
            string source = webclient.DownloadString("https://" + ipaddress + "/index.php/go_campaign_ce/go_barged_in/BARGE/" + AgentSessionID + "/" + palmarino + "/" + ipaddress);
        }
        public void UserInfo(string AgentID)//Function used in 'Dettaglio Agente'; UserInfoResults[];
        {
            var reqparm = new System.Collections.Specialized.NameValueCollection();
            //VARS
            reqparm.Add("userid", AgentID);
            //Send Request
            byte[] responsebytes = webclient.UploadValues("https://" + ipaddress + "/index.php/go_user_ce/userinfo", "POST", reqparm);
            //Decode response byte
            string source = new ASCIIEncoding().GetString(responsebytes);

            //Do Things
            string[] json = source.Split(",".ToCharArray());
            for(int i = 0; i < json.Length; i++)
            {
                json[i] = json[i].Replace("\"", "").Replace("[","").Replace("]","").Replace("{","").Replace("}","");
            }
            UserInfoResults = json;
        }
        public void AgentTalkTimeStatus(string agentID, string dadata, string adata)
        {
            webclient.Credentials = new NetworkCredential(username, userpass);
            string source = webclient.DownloadString("http://" + ipaddress + "/_vicidial_/user_stats.php?&begin_date=" + dadata + "&end_date=" + adata + "&user=" + agentID + "&submit=submit&file_download=1");
            source = source.Replace("\"", "");
            string[] finaldata = source.Split("\n".ToCharArray());
            AgentTalkTime = finaldata;
        }
        public void AgentLoginLogoutStatus(string agentID, string dadata, string adata)
        {
            webclient.Credentials = new NetworkCredential(username, userpass);
            string source = webclient.DownloadString("http://" + ipaddress + "/_vicidial_/user_stats.php?&begin_date=" + dadata + "&end_date=" + adata + "&user=" + agentID + "&submit=submit&file_download=2");
            source = source.Replace("\"", "");
            string[] finaldata = source.Split("\n".ToCharArray());
            AgentLoginLogout = finaldata;
        }
        public void AgentOutboundsCallsStatus(string agentID, string dadata, string adata)
        {
            webclient.Credentials = new NetworkCredential(username, userpass);
            string source = webclient.DownloadString("http://" + ipaddress + "/_vicidial_/user_stats.php?&begin_date=" + dadata + "&end_date=" + adata + "&user=" + agentID + "&submit=submit&file_download=5");
            source = source.Replace("\"", "");
            string[] finaldata = source.Split("\n".ToCharArray());
            AgentOutboundCalls = finaldata;
        }
        public void AgentManualDialStatus(string agentID, string dadata, string adata)
        {
            webclient.Credentials = new NetworkCredential(username, userpass);
            string source = webclient.DownloadString("http://" + ipaddress + "/_vicidial_/user_stats.php?&begin_date=" + dadata + "&end_date=" + adata + "&user=" + agentID + "&submit=submit&file_download=9");
            source = source.Replace("\"", "");
            string[] finaldata = source.Split("\n".ToCharArray());
            AgentManualDialCalls = finaldata;
        }
        public void AgentRecStatus(string agentID, string dadata, string adata)
        {
            webclient.Credentials = new NetworkCredential(username, userpass);
            string source = webclient.DownloadString("http://" + ipaddress + "/_vicidial_/user_stats.php?&begin_date=" + dadata + "&end_date=" + adata + "&user=" + agentID + "&submit=submit&file_download=8");
            source = source.Replace("\"", "");
            string[] finaldata = source.Split("\n".ToCharArray());
            AgentRecs = finaldata;
        }


        //#########################################################################################################################################
        //##############################################        GESTIONE CAMPAGNE             #####################################################
        //Set Campagne Array with data
        public void GetCampagne()
        {
            int CurIndex = 0;
            //Clear global array
            string source = webclient.DownloadString("https://" + ipaddress + "/reports");
            //Find first id
            int index1 = source.IndexOf("<div id=\"campaign_ids\" class=\"go_campaign_menu\">");
            int index2 = source.IndexOf("</div>", index1);
            //Substract string with campaign
            string campaign = source.Substring(index1, index2 - index1).Replace("\t","");
            //Create array
            string[] campaignArray = campaign.Split("\n".ToCharArray());
            //Extract data
            for(int i = 0; i < campaignArray.Length; i++)
            {
                if(campaignArray[i].Contains("<li class=\"go_campaign_submenu\" style=\"padding: 3px 10px 3px 3px; margin: 0px; white-space: nowrap;\" title=\""))
                {
                    Campagne[CurIndex] = campaignArray[i].Replace("<li class=\"go_campaign_submenu\" style=\"padding: 3px 10px 3px 3px; margin: 0px; white-space: nowrap;\" title=\"", "").Replace("\">", ";").Replace("</li>", "");
                    CurIndex++;
                }
            }
        }
        public void GetCampagneDetiail()
        {
            try
            {
                string source = webclient.DownloadString("https://" + ipaddress + "/index.php/go_campaign_ce/go_campaign_list");
                //Get Table HTML
                int table0 = source.IndexOf("<table id=\"mainTable\" class=\"tablesorter\" border=\"0\" cellpadding=\"1\" cellspacing=\"0\" style=\"width:100%;\">");
                int table1 = source.IndexOf("</table>", table0);
                string table = source.Substring(table0, table1 - table0);
                //Get TBODY
                int tbody0 = table.IndexOf("<tbody>");
                int tbody1 = table.IndexOf("</tbody>");
                string tbody = table.Substring(tbody0, tbody1 - tbody0);
                //Get Number Of Campaign
                int campaignCount = 0;
                int index = 0;
                while (tbody.IndexOf("<tr",index) != -1)
                {
                    if(tbody.IndexOf("<tr",index) != -1)
                    {
                        campaignCount++;
                        index = tbody.IndexOf("<tr", index) + 10;
                    }
                }
                index = 0;
                for(int i = 0; i < campaignCount; i++)
                {
                    if (tbody.IndexOf("<tr", index) != -1)
                    {
                        int tr1 = tbody.IndexOf("<tr", index);
                        int trsub = tbody.IndexOf(";\">", tr1);
                        int tr2 = tbody.IndexOf("/tr>", tr1);
                        index = tr1 + 5;
                        string tr = tbody.Substring(trsub + 4, tr2 - (trsub + 6)).Replace("&nbsp;", "").Replace("<td style=\"border-top:#D0D0D0 dashed 1px;\">", "");
                        tr = tr.Replace("<td style=\"border-top:#D0D0D0 dashed 1px;\"","").Replace("&#150;", "-");
                        string[] trArray = tr.Split("\n".ToCharArray());
                        //Extract Data from Array
                        //Get Campagna ID
                        int campid0 = trArray[0].IndexOf("<span onclick=\"modify('");
                        int campid1 = trArray[0].IndexOf("')\"");
                        string campagnaID = trArray[0].Substring(campid0 + 23, campid1 - (campid0 + 23));
                        //Get Nome Campagna
                        int campnam0 = trArray[1].IndexOf("\">");
                        int campnam1 = trArray[1].IndexOf("</span></td>");
                        string campagnaNome = trArray[1].Substring(campnam0 + 2, campnam1 - (campnam0 + 2));
                        //Get Campagna Stato
                        string campagnaStato = trArray[3].Replace("<span style=\"color:green;font-weight:bold;\">", "").Replace("<span style=\"color:red;font-weight:bold;\">","").Replace("</span></td>","").Replace("<span style=\"color:#F00;font-weight:bold;\">","");
                        //Get Dial Metod
                        string campagnaDialMode = trArray[2].Replace("</td>", "");
                        CampagneDettaglio[i] = campagnaID + ";" + campagnaNome + ";" + campagnaStato + ";" + campagnaDialMode;
                    }
                }
            }
            catch (Exception) { }
            
        }
        public void CancellaCampagna(string CampagnaID)
        {
            webclient.DownloadString("https://" + ipaddress + "/index.php/go_campaign_ce/go_update_campaign_list/delete/" + CampagnaID);
            //Reload Campaign
        }
        public void GetCampagnaSettings(string campagnaID)
        {
            string source = webclient.DownloadString("https://" + ipaddress + "/index.php/go_campaign_ce/go_get_settings/" + campagnaID);
            int json0 = source.IndexOf("var testVar = jQuery.parseJSON('{");
            int json1 = source.IndexOf("}');", json0);
            string jsondata = source.Substring(json0 + 33, json1 - (json0 + 33)).Replace("\"","");
            CampagnaSettings = jsondata.Split(",".ToCharArray());
        }
        public void ChangeCampagnaStatus(string CampagnaID,bool Stato)
        {
            string NomeCampagna = CampagnaSettings[0].Split(":".ToCharArray())[1];
            string DialMethod = CampagnaSettings[3].Split(":".ToCharArray())[1];
            string AutoDialLevel = CampagnaSettings[4].Split(":".ToCharArray())[1];
            string CampaignScript = CampagnaSettings[5].Split(":".ToCharArray())[1];
            if(CampaignScript == "null") { CampaignScript = ""; }
            string NumeroInUscita = CampagnaSettings[6].Split(":".ToCharArray())[1];
            if(NumeroInUscita == "" || NumeroInUscita == null) { NumeroInUscita = ""; }
            string CampaignRecording = CampagnaSettings[7].Split(":".ToCharArray())[1];
            string CampaignSegreteria = CampagnaSettings[9].Split(":".ToCharArray())[1];
            string CampaignLocalTime = CampagnaSettings[10].Split(":".ToCharArray())[1];
            string CampaignDescription = CampagnaSettings[1].Split(":".ToCharArray())[1];
            string DialPrefix = CampagnaSettings[43].Split(":".ToCharArray())[1];
            string CampagnaActive = CampagnaSettings[15].Split(":".ToCharArray())[1];
            if (Stato == true) { CampagnaActive = "Y"; } else { CampagnaActive = "N"; }
            if(CampagnaActive == "" || CampagnaActive == null) { CampagnaActive = "Y"; }
            webclient.DownloadString("https://" + ipaddress + "/index.php/go_campaign_ce/go_modify_settings/" + CampagnaID + "/modify/" + NomeCampagna + "," + DialMethod + ",ADVANCE," + AutoDialLevel + "," + CampaignScript + "," + NumeroInUscita + "," + CampaignRecording + "," + CampaignSegreteria + "," + CampaignLocalTime + "," + CampaignDescription + ",,CUSTOM_" + DialPrefix + "," + CampagnaActive + "//0/////");
        }
        public void ChangeCampagnaNome(string CampagnaID, string Nome)
        {
            string NomeCampagna = CampagnaSettings[0].Split(":".ToCharArray())[1];
            string DialMethod = CampagnaSettings[3].Split(":".ToCharArray())[1];
            string AutoDialLevel = CampagnaSettings[4].Split(":".ToCharArray())[1];
            string CampaignScript = CampagnaSettings[5].Split(":".ToCharArray())[1];
            if (CampaignScript == "null") { CampaignScript = ""; }
            string NumeroInUscita = CampagnaSettings[6].Split(":".ToCharArray())[1];
            if (NumeroInUscita == "" || NumeroInUscita == null) { NumeroInUscita = ""; }
            string CampaignRecording = CampagnaSettings[7].Split(":".ToCharArray())[1];
            string CampaignSegreteria = CampagnaSettings[9].Split(":".ToCharArray())[1];
            string CampaignLocalTime = CampagnaSettings[10].Split(":".ToCharArray())[1];
            string CampaignDescription = CampagnaSettings[1].Split(":".ToCharArray())[1];
            string DialPrefix = CampagnaSettings[43].Split(":".ToCharArray())[1];
            string CampagnaActive = CampagnaSettings[15].Split(":".ToCharArray())[1];
            NomeCampagna = Nome;
            webclient.DownloadString("https://" + ipaddress + "/index.php/go_campaign_ce/go_modify_settings/" + CampagnaID + "/modify/" + NomeCampagna + "," + DialMethod + ",ADVANCE," + AutoDialLevel + "," + CampaignScript + "," + NumeroInUscita + "," + CampaignRecording + "," + CampaignSegreteria + "," + CampaignLocalTime + "," + CampaignDescription + ",,CUSTOM_" + DialPrefix + "," + CampagnaActive + "//0/////");
        }
        public void ChangeCampagnaVelocita(string CampagnaID, string Velocita)
        {
            string NomeCampagna = CampagnaSettings[0].Split(":".ToCharArray())[1];
            string DialMethod = CampagnaSettings[3].Split(":".ToCharArray())[1];
            string AutoDialLevel = CampagnaSettings[4].Split(":".ToCharArray())[1];
            string CampaignScript = CampagnaSettings[5].Split(":".ToCharArray())[1];
            if (CampaignScript == "null") { CampaignScript = ""; }
            string NumeroInUscita = CampagnaSettings[6].Split(":".ToCharArray())[1];
            if (NumeroInUscita == "" || NumeroInUscita == null) { NumeroInUscita = ""; }
            string CampaignRecording = CampagnaSettings[7].Split(":".ToCharArray())[1];
            string CampaignSegreteria = CampagnaSettings[9].Split(":".ToCharArray())[1];
            string CampaignLocalTime = CampagnaSettings[10].Split(":".ToCharArray())[1];
            string CampaignDescription = CampagnaSettings[1].Split(":".ToCharArray())[1];
            string DialPrefix = CampagnaSettings[43].Split(":".ToCharArray())[1];
            string CampagnaActive = CampagnaSettings[15].Split(":".ToCharArray())[1];
            AutoDialLevel = Velocita;
            webclient.DownloadString("https://" + ipaddress + "/index.php/go_campaign_ce/go_modify_settings/" + CampagnaID + "/modify/" + NomeCampagna + "," + DialMethod + ",ADVANCE," + AutoDialLevel + "," + CampaignScript + "," + NumeroInUscita + "," + CampaignRecording + "," + CampaignSegreteria + "," + CampaignLocalTime + "," + CampaignDescription + ",,CUSTOM_" + DialPrefix + "," + CampagnaActive + "//0/////");
        }
        public void ChangeCampagnaSegreteria(string CampagnaID, string Segreteria)
        {
            string NomeCampagna = CampagnaSettings[0].Split(":".ToCharArray())[1];
            string DialMethod = CampagnaSettings[3].Split(":".ToCharArray())[1];
            string AutoDialLevel = CampagnaSettings[4].Split(":".ToCharArray())[1];
            string CampaignScript = CampagnaSettings[5].Split(":".ToCharArray())[1];
            if (CampaignScript == "null") { CampaignScript = ""; }
            string NumeroInUscita = CampagnaSettings[6].Split(":".ToCharArray())[1];
            if (NumeroInUscita == "" || NumeroInUscita == null) { NumeroInUscita = ""; }
            string CampaignRecording = CampagnaSettings[7].Split(":".ToCharArray())[1];
            string CampaignSegreteria = CampagnaSettings[9].Split(":".ToCharArray())[1];
            string CampaignLocalTime = CampagnaSettings[10].Split(":".ToCharArray())[1];
            string CampaignDescription = CampagnaSettings[1].Split(":".ToCharArray())[1];
            string DialPrefix = CampagnaSettings[43].Split(":".ToCharArray())[1];
            string CampagnaActive = CampagnaSettings[15].Split(":".ToCharArray())[1];
            CampaignSegreteria = Segreteria;
            webclient.DownloadString("https://" + ipaddress + "/index.php/go_campaign_ce/go_modify_settings/" + CampagnaID + "/modify/" + NomeCampagna + "," + DialMethod + ",ADVANCE," + AutoDialLevel + "," + CampaignScript + "," + NumeroInUscita + "," + CampaignRecording + "," + CampaignSegreteria + "," + CampaignLocalTime + "," + CampaignDescription + ",,CUSTOM_" + DialPrefix + "," + CampagnaActive + "//0/////");
        }





        //#########################################################################################################################################
        //##############################################           GESTIONE LISTE             #####################################################
        public void GetListe()//ListID + ";" + ListName + ";" + ListStatus + ";" + ListLastCall + ";" + ListLenght + ";" + ListCampaign;
        {
            string source = webclient.DownloadString("https://" + ipaddress + "/go_list");
            int listind0 = source.IndexOf("<!-- LISTs TAB -->");
            int listind1 = source.IndexOf("<!-- end view -->");
            string Liste = source.Substring(listind0 + 18, listind1 - (listind0 + 18)).Replace("\r","").Replace("\t","").Replace(" ","");

            //Try different pages
            int page = 1;
            int resultindex = -1;
            while (!Liste.Contains("Norecord(s)found!"))
            {
                source = webclient.DownloadString("https://" + ipaddress + "/go_list/go_list/lists/" + page);
                listind0 = source.IndexOf("<!-- LISTs TAB -->");
                listind1 = source.IndexOf("<!-- end view -->");
                Liste = source.Substring(listind0 + 18, listind1 - (listind0 + 18)).Replace("\r", "").Replace("\t", "").Replace(" ", "");
                //Cicle for get data
                for (int count = 0; count != Liste.Length || count != -1; count = Liste.IndexOf("<!--<divclass=\"rightdivtoolTip\"title=\"MODIFY", count + 20))
                {
                    if (count == -1) { break; }
                    if (count != 0)
                    {
                        int listid1 = Liste.IndexOf("\">-->", count + 20);
                        //Get ListID
                        string ListID = Liste.Substring(count + 44, listid1 - (count + 44));
                        //Get List Name
                        int listn0 = Liste.IndexOf("<tdcolspan=\"\"style=\"padding-bottom:-1px;\">\n", count);
                        int listn1 = Liste.IndexOf("</td>", listn0);
                        string ListName = Liste.Substring(listn0 + 44, listn1 - (listn0 + 44));
                        //Get List Status
                        int liststat0 = Liste.IndexOf("<b><fontcolor=", count);
                        int liststat1 = Liste.IndexOf(">", liststat0 + 10);
                        string ListStatus = Liste.Substring(liststat0 + 14, liststat1 - (liststat0 + 14));
                        if (ListStatus == "red") { ListStatus = "NON ATTIVA"; }
                        if (ListStatus == "green") { ListStatus = "ATTIVA"; }
                        //Get List Last Call
                        int lastcall0 = Liste.IndexOf("<tdalign=\"left\"style=\"padding-bottom:-1px;\">", liststat1);
                        int lastcall1 = Liste.IndexOf("</td>", lastcall0);
                        string ListLastCall = Liste.Substring(lastcall0 + 45, lastcall1 - (lastcall0 + 45));
                        if (ListLastCall == "&nbsp;") { ListLastCall = "Nessuna"; }
                        ListLastCall = ListLastCall.Replace("&#150;", "-").Replace("&nbsp;", "");
                        //Get List Number Lenght
                        int listlen0 = Liste.IndexOf("<tdalign=\"left\"style=\"padding-bottom:-1px;\"><fontcolor=\"RED\"><b>", count);
                        int listlen1 = Liste.IndexOf("</b>", listlen0 + 60);
                        string ListLenght = Liste.Substring(listlen0 + 64, listlen1 - (listlen0 + 64));
                        //Get List Campaign
                        int listcam0 = Liste.IndexOf("<tdalign=\"left\"style=\"padding-bottom:-1px;\">", listlen1);
                        int listcam1 = Liste.IndexOf("&nbsp;</td>", listcam0);
                        string ListCampaign = Liste.Substring(listcam0 + 44, listcam1 - (listcam0 + 44));

                        //Set Array with results
                        Lists[resultindex] = ListID + ";" + ListName + ";" + ListStatus + ";" + ListLastCall + ";" + ListLenght + ";" + ListCampaign;
                    }
                    resultindex++;
                }
                page++;
            }
            Lists = Lists.Where(c => c != null).ToArray();
            string[] snerf = Lists;
            LastListId = Lists[Lists.Length - 1].Split(";".ToCharArray())[0];
        }
        //Get List Detailed Information
        public void GetListDetails(string ListaID)
        {
            var reqparm = new System.Collections.Specialized.NameValueCollection();
            reqparm.Add("items", "showval=" + ListaID);
            reqparm.Add("action", "editlist");
            Byte[] responsebytes = webclient.UploadValues("https://" + ipaddress + "/index.php/go_list/editview", "POST", reqparm);
            string source = ASCIIEncoding.ASCII.GetString(responsebytes);
            source = source.Substring(0, source.IndexOf("##"));
            string[] sourceArray = source.Split(new string[] { "--" }, StringSplitOptions.None);
            //Get List Detail
            string ListID = sourceArray[1];
            string ListName = sourceArray[2];
            string ListCampaign = sourceArray[3];
            string ListStatus = sourceArray[4];
            string ListDescr = sourceArray[5];
            string DateNow = sourceArray[6];
            string ListLastCall = sourceArray[7];
            string ListScadenza = sourceArray[8];
            string ListCID = sourceArray[10];
            string Num1 = sourceArray[13];
            string Num2 = sourceArray[14];
            string Num3 = sourceArray[15];
            string Num4 = sourceArray[16];
            string Num5 = sourceArray[17];
            string ListWebForm = sourceArray[18];
            ListDetail = ListaID + ";" + ListName + ";" + ListCampaign + ";" + ListStatus + ";" + ListDescr + ";" + DateNow + ";" + ListLastCall + ";" + ListScadenza + ";" + ListCID + ";" + Num1 + ";" + Num2 + ";" + Num3 + ";" + Num4 + ";" + Num5 + ";" + ListWebForm;
        }
        public void ChangeListStatus(string ListaID,string status)
        {
            GetListDetails(ListaID);
            string[] Settings = ListDetail.Split(";".ToCharArray());
            //Add Parameter to POST DATA
            var reqparm = new System.Collections.Specialized.NameValueCollection();
            string ListID = ListaID;
            string ListName = Settings[1];
            string ListCampaign = Settings[2];
            string ListStatus = Settings[3];
            string ListDescr = Settings[4];
            string ListScadenza = Settings[7];
            string ListCID = Settings[8];
            string Num1 = Settings[9];
            string Num2 = Settings[10];
            string Num3 = Settings[11];
            string Num4 = Settings[12];
            string Num5 = Settings[13];
            string ListWebForm = Settings[14];
            string agent_script_override = "";
            string drop_inbound_group_override = "";
            string resetList = "N";
            //Active/Deactive
            ListStatus = status;
            string parametri1 = "editlist=editlist&editval=&showvaledit=" + ListaID + "&list_name=" + ListName + "&list_description=" + ListDescr + "&campaign_id=" + ListCampaign + "&reset_time=" + ListScadenza + "&reset_list=" + resetList + "&active=" + ListStatus + "&agent_script_override=" + agent_script_override + "&campaign_cid_override=" + ListCID + "&drop_inbound_group_override=" + drop_inbound_group_override + "&web_form_address=" + ListWebForm + "&xferconf_a_number=" + Num1 + "&xferconf_d_number=" + Num4 + "&xferconf_b_number=" + Num2 + "&xferconf_e_number=" + Num5 + "&xferconf_c_number=" + Num3;
            reqparm.Add("itemsumit", parametri1);
            reqparm.Add("action", "editlistfinal");
            Byte[] Response = webclient.UploadValues("https://" + ipaddress + "/index.php/go_list/editsubmit","POST", reqparm);
        }
        public void ResettaLista(string ListaID)
        {
            GetListDetails(ListaID);
            string[] Settings = ListDetail.Split(";".ToCharArray());
            //Add Parameter to POST DATA
            var reqparm = new System.Collections.Specialized.NameValueCollection();
            string ListID = ListaID;
            string ListName = Settings[1];
            string ListCampaign = Settings[2];
            string ListStatus = Settings[3];
            string ListDescr = Settings[4];
            string ListScadenza = Settings[7];
            string ListCID = Settings[8];
            string Num1 = Settings[9];
            string Num2 = Settings[10];
            string Num3 = Settings[11];
            string Num4 = Settings[12];
            string Num5 = Settings[13];
            string ListWebForm = Settings[14];
            string agent_script_override = "";
            string drop_inbound_group_override = "";
            string resetList = "Y";//Resetta Esiti
            string parametri1 = "editlist=editlist&editval=&showvaledit=" + ListaID + "&list_name=" + ListName + "&list_description=" + ListDescr + "&campaign_id=" + ListCampaign + "&reset_time=" + ListScadenza + "&reset_list=" + resetList + "&active=" + ListStatus + "&agent_script_override=" + agent_script_override + "&campaign_cid_override=" + ListCID + "&drop_inbound_group_override=" + drop_inbound_group_override + "&web_form_address=" + ListWebForm + "&xferconf_a_number=" + Num1 + "&xferconf_d_number=" + Num4 + "&xferconf_b_number=" + Num2 + "&xferconf_e_number=" + Num5 + "&xferconf_c_number=" + Num3;
            reqparm.Add("itemsumit", parametri1);
            reqparm.Add("action", "editlistfinal");
            Byte[] Response = webclient.UploadValues("https://" + ipaddress + "/index.php/go_list/editsubmit", "POST", reqparm);
        }
        public void ChangeListCampaign(string ListaID,string CampaignID)
        {
            if (CampaignID.Contains(";")) { CampaignID = CampaignID.Split(";".ToCharArray())[0]; }
            GetListDetails(ListaID);
            string[] Settings = ListDetail.Split(";".ToCharArray());
            //Add Parameter to POST DATA
            var reqparm = new System.Collections.Specialized.NameValueCollection();
            string ListID = ListaID;
            string ListName = Settings[1];
            string ListCampaign = Settings[2];
            string ListStatus = Settings[3];
            if(ListStatus == "") { ListStatus = "Y"; }
            string ListDescr = Settings[4];
            string ListScadenza = Settings[7];
            string ListCID = Settings[8];
            string Num1 = Settings[9];
            string Num2 = Settings[10];
            string Num3 = Settings[11];
            string Num4 = Settings[12];
            string Num5 = Settings[13];
            string ListWebForm = Settings[14];
            string agent_script_override = "";
            string drop_inbound_group_override = "";
            string resetList = "N";//Resetta Esiti no
            ListCampaign = CampaignID;
            string parametri1 = "editlist=editlist&editval=&showvaledit=" + ListaID + "&list_name=" + ListName + "&list_description=" + ListDescr + "&campaign_id=" + ListCampaign + "&reset_time=" + ListScadenza + "&reset_list=" + resetList + "&active=" + ListStatus + "&agent_script_override=" + agent_script_override + "&campaign_cid_override=" + ListCID + "&drop_inbound_group_override=" + drop_inbound_group_override + "&web_form_address=" + ListWebForm + "&xferconf_a_number=" + Num1 + "&xferconf_d_number=" + Num4 + "&xferconf_b_number=" + Num2 + "&xferconf_e_number=" + Num5 + "&xferconf_c_number=" + Num3;
            reqparm.Add("itemsumit", parametri1);
            reqparm.Add("action", "editlistfinal");
            Byte[] Response = webclient.UploadValues("https://" + ipaddress + "/index.php/go_list/editsubmit", "POST", reqparm);
        }
        public void CaricaNumeri(string _filepath, string _phone_number, string _first_name, string _last_name,string _address1, string _city, string _state, string _province,string _postal_code, string _alt_phone, string _email, string _comments)
        {
            webclient.SetTimeout(99999);
            //Data Vars Settings
            string boundary = "---------------------------" + (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
            string ContentTypeBlank = "\r\n";
            string lineTerm = "\r\n";
            var fileData = webclient.Encoding.GetString(System.IO.File.ReadAllBytes(_filepath));
            string leadsloadVal = "ok";
            string tabvalselVal = "";
            string leadfile_nameVal = @"C:\fakepath\lista pulita.xlsx";
            string list_id_overrideVal = LastListId;//Required
            string phone_code_overrideVal = "39";//Required
            string dupcheckVal = "DUPLIST";//Required, check duplicates in listID
            string postalgmtVal = "AREA";
            string submit_fileVal = "CARICA NUMERI";
            string vendor_lead_code_fieldVal = "-1";
            string source_id_fieldVal = "-1";
            string phone_number_fieldVal = _phone_number;
            string title_fieldVal = "-1";
            string first_name_fieldVal = _first_name;
            string middle_initial_fieldVal = "-1";
            string last_name_fieldVal = _last_name;
            string address1_fieldVal = _address1;
            string address2_fieldVal = "-1";
            string address3_fieldVal = "-1";
            string city_fieldVal = _city;
            string state_fieldVal = _state;
            string province_fieldVal = _province;
            string postal_code_fieldVal = _postal_code;
            string country_code_fieldVal = "-1";
            string gender_fieldVal = "-1";
            string date_of_birth_fieldVal = "-1";
            string alt_phone_fieldVal = _alt_phone;
            string email_fieldVal = _email;
            string security_phrase_fieldVal = "-1";
            string comments_fieldVal = _comments;
            string rank_fieldVal = "-1";
            string owner_fieldVal = "-1";


            //Data to send
            string leadsloadReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"leadsload\"" + lineTerm + ContentTypeBlank + leadsloadVal + lineTerm + "--";
            //----------
            string tabvalselReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"tabvalsel\"" + lineTerm + ContentTypeBlank + tabvalselVal + lineTerm + "--";
            //----------
            string leadfile_nameReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"leadfile_name\"" + lineTerm + ContentTypeBlank + leadfile_nameVal + lineTerm + "--";
            //----------
            string filedataString = fileData;
            string fileReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"leadfile\"; filename=\"lista pulita.xlsx\"" + lineTerm + "Content-Type: application/vnd.openxmlformats-officedocument.spreadsheetml.sheet\r\n\r\n" + filedataString + lineTerm + "--";
            //----------
            string list_id_overrideReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"list_id_override\"" + lineTerm + ContentTypeBlank + list_id_overrideVal + lineTerm + "--";
            //----------
            string phone_code_overrideReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"phone_code_override\"" + lineTerm + ContentTypeBlank + phone_code_overrideVal + lineTerm + "--";
            //----------
            string dupcheckReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"dupcheck\"" + lineTerm + ContentTypeBlank + dupcheckVal + lineTerm + "--";
            //----------
            string postalgmtReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"postalgmt\"" + lineTerm + ContentTypeBlank + postalgmtVal + lineTerm + "--";
            //----------
            string submit_fileReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"submit_file\"" + lineTerm + ContentTypeBlank + submit_fileVal + lineTerm + "--";


            //Setting Header Request
            webclient.Headers.Add("Accept", "*/*");
            webclient.Headers.Add("Accept-Language", "it-IT,it;q=0.8,en-US;q=0.5,en;q=0.3");
            webclient.Headers.Add("Accept-Encoding", "gzip, deflate");
            webclient.Headers.Add("Content-Type", "multipart/form-data; boundary=" + boundary);
            webclient.Headers.Add("X-Requested-With", "XMLHttpRequest");
            Uri uri = new Uri("http://" + ipaddress + "/go_list");
            string data = "--" + leadsloadReq + tabvalselReq + leadfile_nameReq + fileReq + list_id_overrideReq + phone_code_overrideReq + dupcheckReq + postalgmtReq + submit_fileReq + boundary + "--\r\n";
            var datacollection = webclient.Encoding.GetBytes(data);
            var result = webclient.UploadData(uri,"POST",datacollection);



            //SECONDA FASE DEL CARICAMENTO
            boundary = "---------------------------" + (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
            webclient.Headers.Add("Content-Type", "multipart/form-data; boundary=" + boundary);
            //---Settings data Part
            leadsloadReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"leadsload\"" + lineTerm + ContentTypeBlank + "okfinal" + lineTerm + "--";
            string lead_fileReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"lead_file\"" + lineTerm + ContentTypeBlank + "/tmp/admin_listapulita.txt" + lineTerm + "--";
            string leadfileReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"leadfile\"" + lineTerm + ContentTypeBlank + "Array" + lineTerm + "--";
            list_id_overrideReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"list_id_override\"" + lineTerm + ContentTypeBlank + list_id_overrideVal + lineTerm + "--";
            phone_code_overrideReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"phone_code_override\"" + lineTerm + ContentTypeBlank + phone_code_overrideVal + lineTerm + "--";
            dupcheckReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"dupcheck\"" + lineTerm + ContentTypeBlank + dupcheckVal + lineTerm + "--";
            leadfile_nameReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"leadfile_name\"" + lineTerm + ContentTypeBlank + leadfile_nameVal + lineTerm + "--";
            string superfinalReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"superfinal\"" + lineTerm + ContentTypeBlank + lineTerm + "--";
            string vendor_lead_code_fieldReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"vendor_lead_code_field\"" + lineTerm + ContentTypeBlank + vendor_lead_code_fieldVal + lineTerm + "--";
            string source_id_fieldReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"source_id_field\"" + lineTerm + ContentTypeBlank + source_id_fieldVal + lineTerm + "--";
            string phone_number_fieldReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"phone_number_field\"" + lineTerm + ContentTypeBlank + phone_number_fieldVal + lineTerm + "--";
            string title_fieldReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"title_field\"" + lineTerm + ContentTypeBlank + title_fieldVal + lineTerm + "--";
            string first_name_fieldReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"first_name_field\"" + lineTerm + ContentTypeBlank + first_name_fieldVal + lineTerm + "--";
            string middle_initial_fieldReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"middle_initial_field\"" + lineTerm + ContentTypeBlank + middle_initial_fieldVal + lineTerm + "--";
            string last_name_fieldReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"last_name_field\"" + lineTerm + ContentTypeBlank + last_name_fieldVal + lineTerm + "--";
            string address1_fieldReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"address1_field\"" + lineTerm + ContentTypeBlank + address1_fieldVal + lineTerm + "--";
            string address2_fieldReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"address2_field\"" + lineTerm + ContentTypeBlank + address2_fieldVal + lineTerm + "--";
            string address3_fieldReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"address3_field\"" + lineTerm + ContentTypeBlank + address3_fieldVal + lineTerm + "--";
            string city_fieldReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"city_field\"" + lineTerm + ContentTypeBlank + city_fieldVal + lineTerm + "--";
            string state_fieldReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"state_field\"" + lineTerm + ContentTypeBlank + state_fieldVal + lineTerm + "--";
            string province_fieldReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"province_field\"" + lineTerm + ContentTypeBlank + province_fieldVal + lineTerm + "--";
            string postal_code_fieldReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"postal_code_field\"" + lineTerm + ContentTypeBlank + postal_code_fieldVal + lineTerm + "--";
            string country_code_fieldReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"country_code_field\"" + lineTerm + ContentTypeBlank + country_code_fieldVal + lineTerm + "--";
            string gender_fieldReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"gender_field\"" + lineTerm + ContentTypeBlank + gender_fieldVal + lineTerm + "--";
            string date_of_birth_fieldReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"date_of_birth_field\"" + lineTerm + ContentTypeBlank + date_of_birth_fieldVal + lineTerm + "--";
            string alt_phone_fieldReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"alt_phone_field\"" + lineTerm + ContentTypeBlank + alt_phone_fieldVal + lineTerm + "--";
            string email_fieldReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"email_field\"" + lineTerm + ContentTypeBlank + email_fieldVal + lineTerm + "--";
            string security_phrase_fieldReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"security_phrase_field\"" + lineTerm + ContentTypeBlank + security_phrase_fieldVal + lineTerm + "--";
            string comments_fieldReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"comments_field\"" + lineTerm + ContentTypeBlank + comments_fieldVal + lineTerm + "--";
            string rank_fieldReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"rank_field\"" + lineTerm + ContentTypeBlank + rank_fieldVal + lineTerm + "--";
            string owner_fieldReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"owner_field\"" + lineTerm + ContentTypeBlank + owner_fieldVal + lineTerm + "--";
            string OK_to_processReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"OK_to_process\"" + lineTerm + ContentTypeBlank + "PROCEDI" + lineTerm + "--";
            //Complete Data string
            data = "--" + leadsloadReq + lead_fileReq + leadfileReq + list_id_overrideReq + phone_code_overrideReq + dupcheckReq + leadfile_nameReq + superfinalReq + vendor_lead_code_fieldReq + source_id_fieldReq + phone_number_fieldReq + title_fieldReq + first_name_fieldReq + middle_initial_fieldReq + last_name_fieldReq + address1_fieldReq + address2_fieldReq + address3_fieldReq + city_fieldReq + state_fieldReq + province_fieldReq + postal_code_fieldReq + country_code_fieldReq + gender_fieldReq + date_of_birth_fieldReq + alt_phone_fieldReq + email_fieldReq + security_phrase_fieldReq + comments_fieldReq + rank_fieldReq + owner_fieldReq + OK_to_processReq + boundary + "--\r\n";
            datacollection = webclient.Encoding.GetBytes(data);
            result = webclient.UploadData(uri, "POST", datacollection);
            webclient.SetTimeout(0);
        }
        public void CreaLista(string _list_name, string _list_description, string _campaign_id)
        {
            GetListe();
            //Calculate new ListID
            int listid = Convert.ToInt32(LastListId) + 1;
            LastListId = listid.ToString();
            //Do Things
            string urlreq = "https://" + ipaddress + "/go_list";
            string selectVal = "";
            string addSUBMIT = "addSUBMIT";
            string auto_gen = "on";
            string list_id = LastListId;
            string list_name = _list_name;
            string list_description = _list_description;
            string campaign_id = _campaign_id;
            string active = "Y";

            var reqparm = new System.Collections.Specialized.NameValueCollection();
            reqparm.Add("selectval", selectVal);
            reqparm.Add("addSUBMIT", addSUBMIT);
            reqparm.Add("auto_gen", auto_gen);
            reqparm.Add("list_id", list_id);
            reqparm.Add("list_name", list_name);
            reqparm.Add("list_description", list_description);
            reqparm.Add("campaign_id", campaign_id);
            reqparm.Add("active", active);
            Byte[] Response = webclient.UploadValues("https://" + ipaddress + "/index.php/go_list", "POST", reqparm);
        }
        //Get Header From file excel to find number coloumn index
        public void LoadExcelFileHeader(string _filePath)
        {
            //Read Excel File
            Excel.Application xlApp = new Excel.Application();
            Excel.Workbook xlWorkbook = xlApp.Workbooks.Open(_filePath);
            Excel._Worksheet xlWorksheet = xlWorkbook.Sheets[1];
            Excel.Range xlRange = xlWorksheet.UsedRange;
            
            string str;
            int rCnt;
            int cCnt;
            int rw = 0;
            int cl = 0;
            rw = xlRange.Rows.Count;
            cl = xlRange.Columns.Count;
            for (cCnt = 1; cCnt <= cl; cCnt++)
            {
                str = (string)(xlRange.Cells[1, cCnt] as Excel.Range).Value2;
                int index = cCnt - 1;
                ExcelFirstRow[index] = str;
            }
            //CLEANUP
            GC.Collect();
            GC.WaitForPendingFinalizers();
            //rule of thumb for releasing com objects:
            //  never use two dots, all COM objects must be referenced and released individually
            //  ex: [somthing].[something].[something] is bad
            //release com objects to fully kill excel process from running in the background
            System.Runtime.InteropServices.Marshal.ReleaseComObject(xlRange);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(xlWorksheet);
            //close and release
            xlWorkbook.Close();
            System.Runtime.InteropServices.Marshal.ReleaseComObject(xlWorkbook);
            //quit and release
            xlApp.Quit();
            System.Runtime.InteropServices.Marshal.ReleaseComObject(xlApp);
        }
        public void GetListEsiti(string ListaID)
        {
            var reqparm = new System.Collections.Specialized.NameValueCollection();
            reqparm.Add("items", "showval=" + ListaID);
            reqparm.Add("action", "editlist");
            Byte[] responsebytes = webclient.UploadValues("https://" + ipaddress + "/index.php/go_list/editview", "POST", reqparm);
            string source = ASCIIEncoding.ASCII.GetString(responsebytes);
            string Esiti = source.Substring(source.IndexOf("##") + 2, (source.Length - (source.IndexOf("##") + 2)));
            source = source.Substring(0, source.IndexOf("##"));
            string[] sourceArray = source.Split(new string[] { "--" }, StringSplitOptions.None);
            //Check if Esiti contains data
            if(Esiti == "") { ListEsitiArr = new string[] { "Nessun esito in questa lista.  List ID:" + ListaID }; return; }
            //Operation on results
            Esiti = Esiti.Substring(0, Esiti.IndexOf("</center>")).Replace("<tr align=","\n").Replace("<tr class=","\n");
            Esiti = Esiti.Substring(0, Esiti.IndexOf("<center>"));
            Esiti = Esiti.Replace("left class=tr1><td>", "").Replace("left class=tr2><td>", "");
            Esiti = Esiti.Substring(Esiti.IndexOf("<br>CALLED</td><tr>") + 20, Esiti.Length - (Esiti.IndexOf("<br>CALLED</td><tr>") + 20));
            Esiti = Esiti.Replace("</td></tr>", "").Replace("</td><td>",";").Replace("</td><td align=\"center\">", ";");
            Esiti = Esiti.Replace("\"tr2\"><td colspan=2><b>", "").Replace("\"tr1\"><td colspan=2 align=left><b>","");
            Esiti = Esiti.Replace("<b> <font color=\"green\"> ", "").Replace("<b><font color=\"green\">", "");
            Esiti = Esiti.Replace("</td><td colspan=2 align=center><font color=\"blue\"><b>", ";").Replace("</font></table><br><br>", "");
            Esiti = Esiti.Replace("</font>", "").Replace("<b>", "");
            //Finally Make Array with data
            ListEsitiArr = Esiti.Split("\n".ToCharArray());
        }
        public void DownloadList(string ListaID)
        {
            //Choose where to save
            SaveFileDialog saveto = new SaveFileDialog();
            saveto.DefaultExt = ".txt";
            saveto.Filter = "Testo|*.txt";
            saveto.AddExtension = true;
            saveto.ShowDialog();
            //Check save file
            if(saveto.FileName == null || saveto.FileName == "") { return; }
            //Login old interface
            string authInfo = username + ":" + userpass;
            authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));
            webclient.Headers["Authorization"] = "Basic " + authInfo;
            //#################################################################################
            webclient.DownloadFile("https://" + ipaddress + "/_vicidial_/list_download.php?list_id=" + ListaID,saveto.FileName);
        }
        public void CercaNumero(string numero)
        {
            webclient.Credentials = new NetworkCredential(username, userpass);
            var reqparm = new System.Collections.Specialized.NameValueCollection();
            reqparm.Add("archive_search", "No");
            reqparm.Add("phone", numero);
            reqparm.Add("submit", "SUBMIT");
            reqparm.Add("alt_phone_search", "No");
            Byte[] responsebytes = webclient.UploadValues("https://" + ipaddress + "/_vicidial_/admin_search_lead.php", "POST", reqparm);
            string source = ASCIIEncoding.ASCII.GetString(responsebytes);

            //Check if results are present
            if(source.Contains("Please go back and double check the information you entered and submit again"))
            {
                SearchResults[0] = "Nessun Risultato";
                return;
            }
            else
            {
                //Make some operation on output
                int res0 = source.IndexOf("<b>RESULTS: ");
                int res1 = source.IndexOf("</TABLE>", res0);
                source = source.Substring(res0, res1 - res0);
                //some operation
                res0 = source.IndexOf("</TR>");
                source = source.Substring(res0 + 6, source.Length - (res0 + 7));

                source = source.Replace("<TR bgcolor=\"#B9CBFD\">\n", "").Replace("<TR bgcolor=\"#9BB9FB\">\n", "").Replace("<TR bgcolor=\"#B9CBFD\">", "").Replace("<TR bgcolor=\"#9BB9FB\">", "");
                source = source.Substring(0, source.Length - 6);

                source = source.Replace("<TD ALIGN=LEFT><FONT FACE=\"ARIAL,HELVETICA\" SIZE=1>", "").Replace("<TD ALIGN=CENTER><FONT FACE=\"ARIAL,HELVETICA\" SIZE=1><a href=\"admin_modify_lead.php?lead_id=", "").Replace("&archive_search=No\" target=\"_blank\">", ",");
                source = source.Replace("</a></FONT>", "").Replace("<TD ALIGN=CENTER><FONT FACE=\"ARIAL,HELVETICA\" SIZE=1>", "").Replace("</FONT>", "");
                source = source.Replace("</TR>", ";").Replace("</TD>\n", ",").Replace("</TD>", "").Replace("\n", "");
                //write array
                SearchResults = source.Split(";".ToCharArray());
            }
        }




        //#########################################################################################################################################
        //##############################################           GESTIONE ESITI             #####################################################
        public void MoveLeads(string _listIDFrom, string _ListIDTo, string _moveStatus)
        {
            //Login old interface
            string authInfo = username + ":" + userpass;
            authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));
            webclient.Headers["Authorization"] = "Basic " + authInfo;
            //#######################################################
            string ListIDFrom = _listIDFrom;
            string ListIDTo = _ListIDTo;
            string moveStatus = _moveStatus;
            string moveOp = "<";
            string moveCount = "20";
            //Setting Up web POST Request
            var reqparm = new System.Collections.Specialized.NameValueCollection();
            reqparm.Add("move_from_list", ListIDFrom);
            reqparm.Add("move_to_list", ListIDTo);
            reqparm.Add("move_status", moveStatus);
            reqparm.Add("move_count_op", moveOp);
            reqparm.Add("move_count_num", moveCount);
            reqparm.Add("confirm_move", "confirm");
            Byte[] Response = webclient.UploadValues("http://" + ipaddress + "/_vicidial_/lead_tools.php", "POST", reqparm);
            string data = ASCIIEncoding.ASCII.GetString(Response);
        }
        public void UpdateLeads(string _listID,string _fromStatus, string _toStatus)
        {
            //Login old interface
            string authInfo = username + ":" + userpass;
            authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));
            webclient.Headers["Authorization"] = "Basic " + authInfo;
            //#######################################################
            string ListID = _listID;
            string fromStatus = _fromStatus;
            string toStatus = _toStatus;
            string moveOp = "<";
            string moveCount = "20";
            //Setting Up web POST Request
            var reqparm = new System.Collections.Specialized.NameValueCollection();
            reqparm.Add("update_list", ListID);
            reqparm.Add("update_from_status", fromStatus);
            reqparm.Add("update_to_status", toStatus);
            reqparm.Add("update_count_op", moveOp);
            reqparm.Add("update_count_num", moveCount);
            reqparm.Add("confirm_update", "confirm");
            Byte[] Response = webclient.UploadValues("http://" + ipaddress + "/_vicidial_/lead_tools.php", "POST", reqparm);
            string data = ASCIIEncoding.ASCII.GetString(Response);
        }




        //Get Server Status
        public void GetServerInfo()
        {
            //Get Vitals Data
            string source = webclient.DownloadString("https://" + ipaddress + "/application/views/phpsysinfo/vitals.php").Replace("\t","");
            string[] SourceArray = source.Split("\n".ToCharArray());
            int host0 = SourceArray[8].IndexOf("<td class=\"tabdata\">");
            int host1 = SourceArray[8].IndexOf("</font></td>", host0);
            Hostname = SourceArray[8].Substring(host0 + 20, host1 - (host0 + 20));
            //Listening IP
            int listip0 = SourceArray[14].IndexOf("<td class=\"tabdata\">");
            int listip1 = SourceArray[14].IndexOf("</font></td>", listip0);
            ListeningIP = SourceArray[14].Substring(listip0 + 20, listip1 - (listip0 + 20));
            //Kernel Version
            int kerver0 = SourceArray[20].IndexOf("<td class=\"tabdata\" >");
            int kerver1 = SourceArray[20].IndexOf("</font></td>", kerver0);
            KernelVersion = SourceArray[20].Substring(kerver0 + 21, kerver1 - (kerver0 + 21));
            //Distro Name
            int dname0 = SourceArray[26].IndexOf("&nbsp;");
            DistroName = SourceArray[26].Substring(dname0 + 6, SourceArray[26].Length - (dname0 + 6));
            //Uptime
            int upt0 = SourceArray[33].IndexOf("<td class=\"tabdata\">");
            int upt1 = SourceArray[33].IndexOf("</font></td>", upt0);
            Uptime = SourceArray[33].Substring(upt0 + 20, upt1 - (upt0 + 20)).Replace("&nbsp;", " ");
            //Load Averages
            int loada0 = SourceArray[39].IndexOf("&nbsp;");
            int loada1 = SourceArray[39].IndexOf("</font></td>", loada0);
            LoadAverages = SourceArray[39].Substring(loada0 + 6, loada1 - (loada0 + 6));
            //Used Physical Memory (RAM)
            source = webclient.DownloadString("https://" + ipaddress + "/application/views/phpsysinfo/memory.php").Replace("\t","");
            SourceArray = source.Split("\n".ToCharArray());
            int pym0 = SourceArray[25].IndexOf("&nbsp;&nbsp;");
            int pym1 = SourceArray[25].IndexOf("</font></td>", pym0);
            PhysicalMemoryUsed = SourceArray[25].Substring(pym0 + 12, pym1 - (pym0 + 12));
            //Total Disk Usage
            source = webclient.DownloadString("https://" + ipaddress + "/application/views/phpsysinfo/filesystems.php").Replace(" ", "");
            SourceArray = source.Split("\n".ToCharArray());
            int totdisk0 = SourceArray[59].IndexOf("&nbsp;");
            int totdisk1 = SourceArray[59].IndexOf("</font></td>", totdisk0);
            TotalDiskUsage = SourceArray[59].Substring(totdisk0 + 6, totdisk1 - (totdisk0 + 6));
        }
    }
}
