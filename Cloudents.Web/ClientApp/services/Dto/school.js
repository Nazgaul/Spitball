export const School = {
   // Default: function (objInit) {

   // },
   // University: function (objInit) {
   //    if (!objInit) {
   //       this.id = "";
   //       this.country = "";
   //       this.text = "";
   //       this.students = "";
   //       this.image = "";
   //    } else {
   //       this.id = objInit.id;
   //       this.country = objInit.country;
   //       this.text = objInit.name;
   //       this.students = objInit.usersCount || 0;
   //       this.image = objInit.image || '';
   //    }
   // },
   Course: function (objInit) {
      this.text = objInit.name;
      this.isFollowing = objInit.isFollowing || false;
      this.isTeaching = objInit.isTeaching || false;
      this.students = objInit.students || 10;
      this.isPending = objInit.isPending || false;
      this.isLoading = false;
   }
}