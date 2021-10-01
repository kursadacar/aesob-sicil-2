using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using CTD.Core.Constants;
using CTD.Core.Entities;
using CTD.Data.UnitofWork;
using CTD.Dto.SingleDto;
using CTD.Service.Abstracts;
using CTD.Service.Interfaces;
using CTD.Utilities.Manager;

namespace CTD.Service.Services
{
    public class KullaniciService : AKullaniciService, IKullaniciService
    {
        public KullaniciService(IUnitofWork uow) : base(uow)
        {
        }

        public Kullanici KullaniciGetir(string userName, string password)
        {
            var result = _kullaniciRepository.GetAll().SingleOrDefault(p => p.login == userName);

            try
            {
                if (result != null && EnDeCode.Decrypt(result.pass2, StaticParams.SifrelemeParametresi) == password)
                    return result;
            }
            catch
            {
                return null;
            }

            return null;
        }

        public Kullanici KullaniciGetir(int id)
        {
            return _kullaniciRepository.GetAll().FirstOrDefault(k => k.Id == id);
        }

        public Kullanici KullaniciGetir(int id, string pass)
        {
            var user = _kullaniciRepository.GetAll().FirstOrDefault(k => k.Id == id);
            if(EnDeCode.Decrypt(user.pass2, StaticParams.SifrelemeParametresi) == pass)
            {
                return user;
            }
            else
            {
                return null;
            }
        }

        public IEnumerable<Kullanici> IslemYapanKullanicilar()
        {
            var list = (from c in _kullaniciRepository.GetAll()
                where c.YETKILI && c.hak != "muhasebe" && c.sube != "YONETICI"
                select c).ToList().OrderBy(c => c.adi);
            return list;
        }

        public void SifreGuncelle(Kullanici model)
        {
            _kullaniciRepository.Update(model);
        }

        public List<ComboBoxIdTextDto> ComboBoxKullanicilar()
        {
            var result =
                (from c in _kullaniciRepository.GetAll()
                    where c.YETKILI && (c.sube != "YONETICI" || c.hak == "muhasebe")
                    select c).ToList().Select(p => new ComboBoxIdTextDto {id = p.Id.ToString(), text = p.adi}).ToList();
            return result;
        }

        public void IPGuncelle(Kullanici model)
        {
            _kullaniciRepository.Update(model);
        }
    }
}