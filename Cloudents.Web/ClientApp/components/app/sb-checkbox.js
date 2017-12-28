
import checkBox from "vuetify/es5/components/VCheckbox";
export default {
    extends: checkBox,
    computed: {
        icon: function icon() {
            console.log("hi ram", this.isActive,this);
           // if (this.inputIndeterminate) {
               //return 'indeterminate_check_box';
               if (this.isActive) {
                return 'sbf-mic';
              } else {
                return 'sbf-filter';
              }
            }
          
    }
}
