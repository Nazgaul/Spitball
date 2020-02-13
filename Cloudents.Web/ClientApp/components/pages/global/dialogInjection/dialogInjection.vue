<template>
    <component :is="component"></component>
</template>

<script>
import dialogMixin from './dialogMixin.js'
export default {
    mixins: [dialogMixin],
    data: () => ({
        component: ''
    }),
    watch: {
        "$route.query.dialog": "openDialog"
    },
    methods: {
        openDialog(dialogNameFromRoute){
            if(!dialogNameFromRoute){
                this.component = ''
                this.$closeDialog()
                return;
            }else{
                let currentDialogPremissionList = this.dialogsPremissions[dialogNameFromRoute];
                for(let premissionType of currentDialogPremissionList){
                    let result = this.dialogHandlerByType(premissionType,dialogNameFromRoute)
                    if(result === 'break'){
                        this.component = 'break'
                        break;
                    }
                }
                if(this.component === 'break'){
                    this.component = ''
                }else{
                    this.component = dialogNameFromRoute
                }
            }   
        }
    }
}
</script>