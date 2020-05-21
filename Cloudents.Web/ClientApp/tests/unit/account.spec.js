import { mutations } from '../../store/account'

const {
    changeIsUserTutor
    // setIsTutorState,
    // setRefferedNumber,
    // logout,
    // updateUser,
    // changeLoginStatus,
    // setAccountPicture,
    // setUserPendingPayment
} = mutations

describe('ACCOUNT STORE TESTING', () => {

    it('Should change user status to tutor', () => {
        const state = { user: null }

        changeIsUserTutor(state, true)

        expect(state.user.isTutor).toBe(true)
    })


    // it('Should set currency by user locale', () => {
    //     const state = { user: null }

    //     changeIsUserTutor(state, true)

    //     expect(state.user.isTutor).toBe(true)
    // })

})