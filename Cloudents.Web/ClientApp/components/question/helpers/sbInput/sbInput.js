export default {
    props: {
        value: {type: String},
        icon: {
            type: String
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