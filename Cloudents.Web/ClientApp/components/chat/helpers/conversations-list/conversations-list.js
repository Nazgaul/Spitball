export default {
    props:{
        rooms:Array
    },
    data() {
        return {
            activeRoom:null
        }
    },
    methods:{
        selectRoom(room){
            this.activeRoom = room;
            this.$emit('selectRoom', this.activeRoom);
        }
    }
   
}