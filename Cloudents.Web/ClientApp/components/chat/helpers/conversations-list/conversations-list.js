import userBlock from "./../../../helpers/user-block/user-block.vue";

export default {
    components:{userBlock},
    props:{
        rooms:Array        
    },
    data() {
        return {
            activeRoom:null,
            user:{
                img:'https://cdn.pixabay.com/photo/2016/08/20/05/38/avatar-1606916_960_720.png',
                name:'hbhjbj'
            }
        }
    },
    methods:{
        selectRoom(room){
            this.activeRoom = room;
            this.$emit('selectRoom', this.activeRoom);
        }
    }
   
}