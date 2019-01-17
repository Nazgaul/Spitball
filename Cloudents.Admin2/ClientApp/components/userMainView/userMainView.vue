<template>
    
</template>

<script>
    import UserMainService from './userMainService'
    export default {
        name: "userMainView",
        created(){
            UserMainService.getUserData(759).then((success)=>{
            console.log( success, 'donme')
            },
                (error)=>{
                    console.log(error, 'error')
                }

            )
        }
    }

</script>

<style scoped>

</style>