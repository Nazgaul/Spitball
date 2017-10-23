<template>
</template>
<script>
    import { mapActions } from 'vuex'
    export default {
        methods: {
            ...mapActions(['getUniversities','getCorses','createCourse'])
        },
    created(){
        this.getUniversities('').then(({ body})=>{
            console.log(body);
        })
        this.getCorses({ term: '', universityId: 172621 }).then(({ body}) => {
            console.log(body);
        })
        this.createCourse({ name: 'momo'}).then((res) => {
            console.log(res);
        })
    
    }
    }
</script>