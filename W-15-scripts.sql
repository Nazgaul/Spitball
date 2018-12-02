
--Update document university
BEGIN tran
update D
set UniversityId = t.University
from sb.Document D
join (select U2.Id, d.Id
		from sb.Document d 
		join zbox.Item i 
			on d.OldId = i.ItemId
		join zbox.Box b 
			on i.BoxId = b.BoxId
		join Zbox.University u
			on b.University = u.Id
		join sb.University u2
			on u.UniversityName = u2.Name and u.Country = u2.Country) t (University, tId) on t.tId = D.Id
commit