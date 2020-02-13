<template>
    <component :is="component"></component>
</template>

<script>
import dialogComponents from './import.js'
import dialogConfing from './dialogConfing'

export default {
    mixins: [dialogComponents],
    data: () => ({
        component: ''
    }),
    watch: {
        "$route.query.dialog": "openDialog"
    },
    methods: {
        openDialog(component) {
            let dialogName = dialogConfing.getDialog(component);
            if(typeof dialogName === 'object'){
                this.$router.push(dialogName);
                return
            }
            if(dialogName){
                dialogName !== component? this.$openDialog(dialogName) : this.component = dialogName;
            }else{
                this.$closeDialog()
                this.component = ''
            }
        }
    }
}
</script>