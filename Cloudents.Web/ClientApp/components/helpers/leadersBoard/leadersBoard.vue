<template>
<div>

</div>

</template>

<script>
    import leaderBoardService from '../../../services/leadersBoardService'
    export default {
        name: "leadersBoard",
        data() {
            return {
            }
        },
        props: {
            propName: {
                type: Number,
                default: 0
            },
        },
        computed: {
            name() {
                return this;
            }
        },
        methods: {
           getLeaderBoardData(){
               leaderBoardService.getLeaderBoardItems().then((resp)=>{
                   console.log('leaders success', resp)
               },
                   (error)=>{
                       console.log('leaders error', error)
                   })
           }
        },
        created(){
            this.getLeaderBoardData()
        }
    }
</script>

<style lang="less" src="./leadersBoard.less">

</style>