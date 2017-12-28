
import checkBox from "vuetify/es5/components/VCheckbox";
export default {
    extends: checkBox,
    computed: {
        icon: function icon() {
            if (this.isActive) {
                return 'sbf-checkbox-marked';
            } else {
                return 'sbf-checkbox-blank-outline';
            }
        }

    }
}
