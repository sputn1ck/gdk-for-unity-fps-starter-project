using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Bbhrpc;
public interface IAdvertiserStore
{
    Task Initialize(ListAdvertiserResponse advertisers);
    Task<Texture2D> GetAdvertiser(string advertiserId, string advertiserUrl);
}

