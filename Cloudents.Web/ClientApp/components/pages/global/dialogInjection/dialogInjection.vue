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
        "$route.query.dialog": "openDialog",
        component(val){
            if (val) {
                if (this.$vuetify.breakpoint.xs) {
                    document.getElementsByTagName("body")[0].className = "noscroll";
                }
            } else {
                document.body.removeAttribute("class", "noscroll");
            }
        }
    },
    methods: {
        openDialog(dialogNameFromRoute){
            if(!dialogNameFromRoute){
                this.component = ''
                this.$closeDialog()
                return;
            }else{
                let currentDialogPremissionList = this.dialogsPremissions[dialogNameFromRoute];
                let self = this;
                this.$nextTick(()=>{
                    for(let premissionType of currentDialogPremissionList){
                        let result = self.dialogHandlerByType(premissionType,dialogNameFromRoute)
                        if(result === 'break'){
                            self.component = 'break';
                            break;
                        }
                        if(self.component){
                            break;
                        }
                    }

                    if(self.component === 'break'){
                        self.component = ''
                        return
                    }
                    if(self.component){
                        return
                    }else{
                        self.component = dialogNameFromRoute
                        return
                    }
                })
            }   
        }
    }
}
</script>