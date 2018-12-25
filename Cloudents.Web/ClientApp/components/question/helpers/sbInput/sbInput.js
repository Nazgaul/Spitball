export default {
    props: {
        value: {type: String},
        // rules:{},
        icon: {
            type: String,
            default: ''
        },
        prependInnerIcon:{
            type: String,
            default: ''
        },
        errorMessage: {
            type: String
        },
        name: {
            type: String
        },
        type: {
            type: String
        },
        editable: {
            type: Boolean,
            default: true
        },
        placeholder: {
            type: String
        },
        autofocus: {
            type: Boolean,
        },
        focus: {
            type: Boolean,
            default: false
        },
        required:{
            type: Boolean,
            default: false
        },
        disabled:{
            type:Boolean,
            default:false
        },
        bottomError:{
            type: Boolean,
            default: false
        },
        hint:{
            default: '',
            required: false,
            type: ''
        }
    },
    data: function () {
        return {
            isReadOnly: !this.editable
        }
    },
    watch: {
        focus(newVal, oldVal) {
            if (newVal) {
                this.isReadOnly = false;
                this.$el.querySelector('.input-field').focus()
            }
        }
    },
    methods: {
        updateValue: function (value) {
            this.$emit('input', value);
        }
    },
}