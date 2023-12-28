﻿using Newtonsoft.Json;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.X509.Extension;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using YukiDNS.DNS_CORE;
using YukiDNS.CA_CORE;
using YukiDNS.DNS_RFC;

namespace YukiDNS
{
    class Program
    {        

        static void Main(string[] args)
        {
            if (args[0] == "dns")
            {
                DNSService.Start();
            }
            else if (args[0] == "zone")
            {
                string[] data = File.ReadAllLines(@"zones\e1_ksyuki_com.zone");

                List<ZoneData> list = new List<ZoneData>();

                foreach (string line in data)
                {
                    if (string.IsNullOrEmpty(line)) continue;
                    var line2 = line.Replace("\t", " ");

                    while(line2.Contains("  "))
                    {
                        line2=line2.Replace("  ", " ");
                    }


                    try
                    {
                        ZoneData data1=ZoneParser.ParseLine(line2,"e1.ksyuki.com");
                        if (data1.Type != QTYPES.RRSIG)
                        {
                            list.Add(data1);
                        }
                        else
                        {
                            var ql = list.Where(q => q.Type == (QTYPES)data1.Data[0] && q.Name == data1.Name).ToList();
                            if (ql.Count > 0)
                            {
                                ql[0].RRSIG = data1;
                            }
                        }
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex.Message+":"+line2.Split(' ')[3]);
                    }
                }

                foreach (var data1 in list)
                {
                    Console.WriteLine(JsonConvert.SerializeObject(data1));
                }

                Console.ReadLine();
            }
            else
            {
                CA_Program.Main(args.Skip(1).ToArray());
            }
        }       
    }
}
