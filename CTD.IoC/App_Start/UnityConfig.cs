using System;
using System.Globalization;
using System.Threading;
using System.Web.Mvc;
using CTD.Core.Entities;
using CTD.Data.GenericRepository;
using CTD.Data.UnitofWork;
using CTD.Service.Interfaces;
using CTD.Service.Services;
using Microsoft.Practices.Unity;
using Unity.Mvc5;

namespace CTD.IoC
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
            CultureInfo culture = (CultureInfo)CultureInfo.CreateSpecificCulture("tr-TR");
            culture.DateTimeFormat.ShortDatePattern = "dd/MMM/yyyy";
            culture.DateTimeFormat.LongTimePattern = "";
            Thread.CurrentThread.CurrentCulture = culture;

            var container = new UnityContainer();
            RegisterTypes(container);
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }

        public static void RegisterTypes(IUnityContainer container)
        {
            container.BindInRequestScope<IUnitofWork, UnitofWork>();
            container.BindInRequestScope<IKullaniciService, KullaniciService>();
            container.BindInRequestScope<ISicilService, SicilService>();
            container.BindInRequestScope<ITanimService, TanimService>();
            container.BindInRequestScope<IMakbuzDokumService, MakbuzDokumService>();
            container.BindInRequestScope<IMakbuzService, MakbuzService>();
            container.BindInRequestScope<IGenericRepository<CadSokBulv>, GenericRepository<CadSokBulv>>();
            container.BindInRequestScope<IGenericRepository<Il>, GenericRepository<Il>>();
            container.BindInRequestScope<IGenericRepository<Ilce>, GenericRepository<Ilce>>();
            container.BindInRequestScope<IGenericRepository<IslemTurleri>, GenericRepository<IslemTurleri>>();
            container.BindInRequestScope<IGenericRepository<Kullanici>, GenericRepository<Kullanici>>();
            container.BindInRequestScope<IGenericRepository<Mahalle>, GenericRepository<Mahalle>>();
            container.BindInRequestScope<IGenericRepository<Makbuz>, GenericRepository<Makbuz>>();
            container.BindInRequestScope<IGenericRepository<MakbuzDetay>, GenericRepository<MakbuzDetay>>();
            container.BindInRequestScope<IGenericRepository<MakbuzDetayLog>, GenericRepository<MakbuzDetayLog>>();
            container.BindInRequestScope<IGenericRepository<MakbuzDokum>, GenericRepository<MakbuzDokum>>();
            container.BindInRequestScope<IGenericRepository<MakbuzDokumLog>, GenericRepository<MakbuzDokumLog>>();
            container.BindInRequestScope<IGenericRepository<Meslekler>, GenericRepository<Meslekler>>();
            container.BindInRequestScope<IGenericRepository<MeslekOdasi>, GenericRepository<MeslekOdasi>>();
            container.BindInRequestScope<IGenericRepository<MeslekTerkNedeni>, GenericRepository<MeslekTerkNedeni>>();
            container.BindInRequestScope<IGenericRepository<Nace>, GenericRepository<Nace>>();
            container.BindInRequestScope<IGenericRepository<Sicil>, GenericRepository<Sicil>>();
            container.BindInRequestScope<IGenericRepository<SicilMeslek>, GenericRepository<SicilMeslek>>();
            container
                .BindInRequestScope<IGenericRepository<SicilMeslekDegisiklik_Log>,
                    GenericRepository<SicilMeslekDegisiklik_Log>>();
            container.BindInRequestScope<IGenericRepository<SicilMeslekLog>, GenericRepository<SicilMeslekLog>>();
            container.BindInRequestScope<IGenericRepository<Sinif>, GenericRepository<Sinif>>();
            container.BindInRequestScope<IGenericRepository<TahsilatGruplari>, GenericRepository<TahsilatGruplari>>();
            container.BindInRequestScope<IGenericRepository<TahsilatIslemleri>, GenericRepository<TahsilatIslemleri>>();
            container.BindInRequestScope<IGenericRepository<TahsilatKalemleri>, GenericRepository<TahsilatKalemleri>>();
            container
                .BindInRequestScope<IGenericRepository<TahsilatKalemleriFiyat>,
                    GenericRepository<TahsilatKalemleriFiyat>>();
            container.BindInRequestScope<IGenericRepository<TahsilatGruplari>, GenericRepository<TahsilatGruplari>>();
        }

        public static void BindInRequestScope<T1, T2>(this IUnityContainer container) where T2 : T1
        {
            container.RegisterType<T1, T2>(new HierarchicalLifetimeManager());
        }
    }
}