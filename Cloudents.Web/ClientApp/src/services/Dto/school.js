export const School = {
   Course: function (objInit) {
      this.text = objInit.name;
      // this.isTeaching = objInit.isTeaching || false;
      // this.students = objInit.students || 10;
      this.isPending = objInit.isPending || false;
      this.isLoading = false;
   }
}