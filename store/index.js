export const state = () => ({
  formSearch: {
    invoiceNo: '',
    lineOperator: '',
  },

  resultTB: [],
})

export const getters = {
  getformSearch(state) {
    return state.formSearch
  },

  getresultTB(dt) {
    return dt.resultTB
  },
}

export const mutations = {
  SET_FORMSEARCH(state, data) {
    state.formSearch = {
      ...state.formSearch,
      ...data,
    }
  },

  SET_RESUALTB(dt, data) {
    dt.resultTB = {
      ...data,
    }
  },
}

export const actions = {
  setformSearch({ commit }, data) {
    commit('SET_FORMSEARCH', data)
  },

  setresultTB({ commit }, data) {
    commit('SET_RESUALTB', data)
  },
}
