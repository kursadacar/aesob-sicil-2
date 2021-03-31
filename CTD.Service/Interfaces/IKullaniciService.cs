﻿using CTD.Core.Entities; using CTD.Dto.SingleDto; using System.Collections.Generic;  namespace CTD.Service.Interfaces { public interface IKullaniciService { Kullanici KullaniciGetir(string userName, string password); Kullanici KullaniciGetir(int id); Kullanici KullaniciGetir(int id, string pass); IEnumerable<Kullanici> IslemYapanKullanicilar(); void SifreGuncelle(Kullanici model); List<ComboBoxIdTextDto> ComboBoxKullanicilar(); void IPGuncelle(Kullanici model); } } 